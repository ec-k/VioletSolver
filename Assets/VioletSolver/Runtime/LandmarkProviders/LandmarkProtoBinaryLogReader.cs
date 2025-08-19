using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HumanLandmarks.Log;
using System;
using System.IO;

namespace VioletSolver.LandmarkProviders
{
    /// <summary>
    /// Reads Protobuf binary log files and provides avatar pose data.
    /// </summary>
    public class LandmarkProtoBinaryLogReader : ILandmarkLogReader
    {
        FileStream _fileStream;
        BinaryReader _binaryReader;
        string _logFilePath;
        bool _isInitialized = false;
        long _startingPointOfBody = 0;
        long _nextFrameOffset = 0;
        double? _firstTimestampMillis = null;
        double _relativeTimeFromStartMs = 0;

        LogFrameData _currentFrameDataBuffer;

        bool _isPlaying = false;

        MessageParser<LogHeader> _logHeaderParser = new MessageParser<LogHeader>(() => new LogHeader());
        MessageParser<LogFrameData> _logFrameDataParser = new MessageParser<LogFrameData>(() => new LogFrameData());

        public event Action<HumanLandmarks.HolisticLandmarks, float> OnLandmarksReceived;

        public float PlaybackSpeed { get; set; } = 1.0f;
        public bool IsPlaying => _isPlaying;
        public LogHeader LogHeader { get; set; }

        /// <summary>
        /// Initializes the log file reader and reads the header.
        /// Provided as a synchronous method due to interface constraints, but performs file opening internally.
        /// </summary>
        /// <param name="logFilePath">The path to the log file.</param>
        /// <returns>True if initialization was successful, otherwise false.</returns>
        public bool Initialize(string logFilePath)
        {
            if (_isInitialized)
            {
                UnityEngine.Debug.LogWarning("LandmarkProtoBinaryLogReader is already initialized.");
                return true;
            }

            _logFilePath = Path.Combine(UnityEngine.Application.persistentDataPath, logFilePath);;

            try
            {
                _fileStream = new FileStream(_logFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                _binaryReader = new BinaryReader(_fileStream);

                if (_binaryReader.BaseStream.Length < 4)
                {
                    UnityEngine.Debug.LogError($"Log file is too short.Cannot read header length prefix: {_logFilePath}");
                    Dispose();
                    return false;
                }
                int headerLength = _binaryReader.ReadInt32();

                if (_binaryReader.BaseStream.Length < _binaryReader.BaseStream.Position + headerLength)
                {
                    UnityEngine.Debug.LogError($"Log file header is corrupted or incomplete: {_logFilePath}");
                    Dispose();
                    return false;
                }
                byte[] headerBytes = _binaryReader.ReadBytes(headerLength);

                LogHeader = _logHeaderParser.ParseFrom(headerBytes);
                if (LogHeader == null)
                {
                    UnityEngine.Debug.LogError($"Failed to deserialize LogHeader: {_logFilePath}");
                    Dispose();
                    return false;
                }

                // Record the initial position for the next frame read.
                _startingPointOfBody = _fileStream.Position;
                _nextFrameOffset = _fileStream.Position;
                _isInitialized = true;
                UnityEngine.Debug.Log("LandmarkProtoBinaryLogReader initialized.");
                return true;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Failed to initialize LandmarkProtoBinaryLogReader: {ex.Message}");
                Dispose();
                return false;
            }
        }

        public void StartPlayback()
        {
            if (!_isInitialized)
            {
                UnityEngine.Debug.LogWarning("Reader is not initialized. Call Initialize() first.");
                return;
            }
            _isPlaying = true;
            _relativeTimeFromStartMs = UnityEngine.Time.timeAsDouble * 1000.0;
            UnityEngine.Debug.Log("Playback started.");
        }

        public void StopPlayback()
        {
            _isPlaying = false;
            UnityEngine.Debug.Log("Playback stopped.");
        }

        public void ResetPlayback()
        {
            if (!_isInitialized) return;
            StopPlayback();
            _fileStream.Seek(_startingPointOfBody, SeekOrigin.Begin); // Return to the start of the body.
            _nextFrameOffset = _startingPointOfBody; // Reset the next frame offset to the start of the body.
            _firstTimestampMillis = null;
            _currentFrameDataBuffer = null;
            UnityEngine.Debug.Log("Playback reset.");
        }

        /// <summary>
        /// Called every frame to read log data and fire events.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last frame in seconds.</param>
        public async UniTask Update(float deltaTime)
        {
            if (!_isInitialized || !_isPlaying) return;


            var currentUnityTimeMs = (UnityEngine.Time.timeAsDouble * 1000.0 * PlaybackSpeed) - _relativeTimeFromStartMs;

            // Current playback time in Unity (milliseconds, considering playback speed).
            // The loop continues to process log frames that should have occurred by this time.
            // This ensures playback catches up even if the log's framerate is higher than Unity's.
            while (_isInitialized && _isPlaying)
            {
                LogFrameData frameToProcess = _currentFrameDataBuffer;
                if (frameToProcess is null)
                {
                    bool _;
                    (_, frameToProcess) = await ReadNextFrameUniTaskAsync().SuppressCancellationThrow();

                    if (frameToProcess is null)
                    {
                        UnityEngine.Debug.Log("End of log file reached.");
                        _isPlaying = false; // Stop playback at the end of the file.
                        _currentFrameDataBuffer = null;
                        break;
                    }

                    // Record the timestamp of the first frame if not already set.
                    if (!_firstTimestampMillis.HasValue)
                    {
                        _firstTimestampMillis = frameToProcess.TimestampMs;
                    }
                }

                // Calculate the relative timestamp from the first frame's timestamp (in milliseconds).
                // This `relativeTimestampMs` indicates the elapsed time from the log recording's start.
                double relativeTimestampMs = frameToProcess.TimestampMs - _firstTimestampMillis.Value;

                if (relativeTimestampMs <= currentUnityTimeMs)
                {
                    OnLandmarksReceived?.Invoke(frameToProcess.HolisticLandmarks, (float)relativeTimestampMs);
                    _currentFrameDataBuffer = null;
                }
                else
                {
                    // This frame is in the future relative to Unity's current time, so buffer it and wait.
                    _currentFrameDataBuffer = frameToProcess;
                    break;
                }
            }
        }

        /// <summary>
        /// Asynchronously reads the next log frame data.
        /// Returns (false, null) if the end of the file is reached or an error occurs.
        /// </summary>
        async UniTask<LogFrameData> ReadNextFrameUniTaskAsync()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("The reader is not initialized. Call Initialize first.");

            // Prevnts Unity from freezing during file read operations.
            await UniTask.Yield(PlayerLoopTiming.Update);

            // Reset the position to the next frame offset
            _fileStream.Seek(_nextFrameOffset, SeekOrigin.Begin);

            // Reached the end of the file or cannot read the next length prefix.
            if (_fileStream.Position + 4 > _fileStream.Length)
                return null;

            byte[] lengthBytes = _binaryReader.ReadBytes(4);
            if (lengthBytes.Length < 4) return null; // If we can't read the length prefix, return null.
            int frameLength = BitConverter.ToInt32(lengthBytes, 0);

            if (_fileStream.Position + frameLength > _fileStream.Length)
            {
                UnityEngine.Debug.LogError($"Incomplete frame data. Expected length: {frameLength}, remaining bytes: {_fileStream.Length - _fileStream.Position}");
                return null;
            }

            byte[] frameBytes = _binaryReader.ReadBytes(frameLength);

            LogFrameData frameData = _logFrameDataParser.ParseFrom(frameBytes);

            // Update position for the next frame read
            _nextFrameOffset = _fileStream.Position;

            return frameData;
        }

        /// <summary>
        /// Releases managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _binaryReader?.Close();
            _binaryReader?.Dispose();
            _fileStream?.Close();
            _fileStream?.Dispose();
            _binaryReader = null;
            _fileStream = null;
            _isInitialized = false;
            _isPlaying = false;
            UnityEngine.Debug.Log("LandmarkProtoBinaryLogReader disposed.");
        }
    }
}