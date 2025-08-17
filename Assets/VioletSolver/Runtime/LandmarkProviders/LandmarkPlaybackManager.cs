using System;
using UnityEngine;
using HumanLandmarks.Log;

namespace VioletSolver.LandmarkProviders
{
    /// <summary>
    /// Unity component managing avatar pose playback.
    /// </summary>
    public class LandmarkPlaybackManager : MonoBehaviour, ILandmarkProvider
    {
        public event Action<HumanLandmarks.HolisticLandmarks, float> OnLandmarksReceived;

        [Tooltip("Path to the log file for playback. Can be an asset path or an absolute path.")]
        public string logFilePath;

        [Tooltip("Playback speed of the log.")]
        [Range(0.1f, 10.0f)]
        public float playbackSpeed = 1.0f;

        ILandmarkLogReader _logFileReader;

        public bool IsPlayingLog => _logFileReader?.IsPlaying ?? false;
        public float CurrentPlaybackSpeed
        {
            get => _logFileReader?.PlaybackSpeed ?? 0f;
            set
            {
                playbackSpeed = value;
                if (_logFileReader != null)
                {
                    _logFileReader.PlaybackSpeed = value;
                }
            }
        }

        public LogHeader LogHeader => _logFileReader?.LogHeader;

        public void Play() => _logFileReader?.StartPlayback();
        public void Pause() => _logFileReader?.StopPlayback();
        public void Rewind() => _logFileReader?.ResetPlayback();

        void Awake()
        {
            _logFileReader = new LandmarkProtoBinaryLogReader();
            _logFileReader.OnLandmarksReceived += OnLogLandmarksReceived;
        }

        void Start()
        {
            bool isInitialized = _logFileReader.Initialize(logFilePath);

            if (isInitialized)
                _logFileReader.PlaybackSpeed = playbackSpeed;
            else
            {
                Debug.LogError($"Failed to initialize log file reader. Path: {logFilePath}");
                // Disable this component if initialization fails to prevent further Update calls.
                enabled = false;
            }
        }

        void Update()
        {
            if (!enabled) return;

            _logFileReader?.Update(Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (IsPlayingLog)
                {
                    _logFileReader?.StopPlayback();
                }
                else
                {
                    _logFileReader?.StartPlayback();
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                _logFileReader?.ResetPlayback();
            }
        }

        void OnLogLandmarksReceived(HumanLandmarks.HolisticLandmarks landmarks, float relativeTime)
        {
            OnLandmarksReceived?.Invoke(landmarks, relativeTime);
        }

        // Release resources
        void OnDestroy()
        {
            if (_logFileReader is not null)
            {
                _logFileReader.OnLandmarksReceived -= OnLogLandmarksReceived;
                _logFileReader.Dispose();
                _logFileReader = null;
            }
        }
    }
}