using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HumanLandmarks.Log;

namespace VioletSolver.LandmarkProviders
{
    /// <summary>
    /// Protobufバイナリログファイルを読み込み、アバター姿勢データを提供するリーダー。
    /// ILandmarkLogReader インターフェースを実装し、UniTask を使用して非同期I/Oを処理する。
    /// </summary>
    public class LandmarkProtoBinaryLogReader : ILandmarkLogReader
    {
        FileStream _fileStream;
        BinaryReader _binaryReader;
        string _logFilePath;
        bool _isInitialized = false;
        long _nextFrameOffset = 0;
        double? _firstTimestampMillis = null;

        LogFrameData _currentFrameDataBuffer;

        bool _isPlaying = false;

        MessageParser<LogHeader> _logHeaderParser = new MessageParser<LogHeader>(() => new LogHeader());
        MessageParser<LogFrameData> _logFrameDataParser = new MessageParser<LogFrameData>(() => new LogFrameData());

        public event Action<HumanLandmarks.HolisticLandmarks, float> OnLandmarksReceived;

        public float PlaybackSpeed { get; set; } = 1.0f;
        public bool IsPlaying => _isPlaying;
        public LogHeader LogHeader { get; private set; }

        /// <summary>
        /// ログファイルを初期化し、ヘッダを読み込む。
        /// インターフェースの制約により同期メソッドとして提供するが、内部でファイルを開く。
        /// </summary>
        /// <param name="logFilePath">ログファイルのパス。</param>
        /// <returns>初期化が成功した場合は true、それ以外は false。</returns>
        public bool Initialize(string logFilePath)
        {
            if (_isInitialized)
            {
                UnityEngine.Debug.LogWarning("LandmarkProtoBinaryLogReader は既に初期化されています。");
                return true;
            }

            _logFilePath = Path.Combine(UnityEngine.Application.persistentDataPath, logFilePath);;

            try
            {
                // FileShare.Read を指定して、他のプロセスが読み取り可能にする
                _fileStream = new FileStream(_logFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                _binaryReader = new BinaryReader(_fileStream);

                // LogHeader を読み込む
                // 4バイトの長さプレフィックスを読み込む
                if (_binaryReader.BaseStream.Length < 4)
                {
                    UnityEngine.Debug.LogError("ログファイルが短すぎます。ヘッダの長さプレフィックスが読み取れません: " + _logFilePath);
                    Dispose();
                    return false;
                }
                int headerLength = _binaryReader.ReadInt32();

                if (_binaryReader.BaseStream.Length < _binaryReader.BaseStream.Position + headerLength)
                {
                    UnityEngine.Debug.LogError("ログファイルのヘッダ部分が破損しているか、不完全です: " + _logFilePath);
                    Dispose();
                    return false;
                }
                byte[] headerBytes = _binaryReader.ReadBytes(headerLength);

                LogHeader = _logHeaderParser.ParseFrom(headerBytes);
                if (LogHeader == null)
                {
                    UnityEngine.Debug.LogError("LogHeader のデシリアライズに失敗しました: " + _logFilePath);
                    Dispose();
                    return false;
                }

                // 最初のフレームデータの位置を記憶する
                _nextFrameOffset = _fileStream.Position;
                _isInitialized = true;
                UnityEngine.Debug.Log("LandmarkProtoBinaryLogReader が初期化されました。");
                return true;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"LandmarkProtoBinaryLogReader の初期化に失敗しました: {ex.Message}");
                Dispose();
                return false;
            }
        }

        public void StartPlayback()
        {
            if (!_isInitialized)
            {
                UnityEngine.Debug.LogWarning("リーダーが初期化されていません。Initialize() を先に呼び出してください。");
                return;
            }
            _isPlaying = true;
            UnityEngine.Debug.Log("再生を開始しました。");
        }

        public void StopPlayback()
        {
            _isPlaying = false;
            UnityEngine.Debug.Log("再生を停止しました。");
        }

        public void ResetPlayback()
        {
            if (!_isInitialized) return;
            StopPlayback(); // 再生を停止
            _fileStream.Seek(_nextFrameOffset, SeekOrigin.Begin); // ヘッダの直後に戻る
            _firstTimestampMillis = null; // 最初のタイムスタンプをリセット
            _currentFrameDataBuffer = null; // バッファをクリア
            UnityEngine.Debug.Log("再生を巻き戻しました。");
        }

        /// <summary>
        /// 毎フレーム呼び出され、ログデータを読み込み、イベントを発火する。
        /// </summary>
        /// <param name="deltaTime">前フレームからの経過時間 (秒)。</param>
        public async UniTask Update(float deltaTime) // async void はエラーハンドリングに注意が必要
        {
            if (!_isInitialized || !_isPlaying) return;

            // リアルタイムと再生速度に基づいてターゲット再生時間を進める
            // deltaTime は秒なので、ミリ秒に変換
            // PlaybackSpeed はここで乗算し、GetRelativeTimestampでは純粋な時間差を扱う
            double currentUnityTimeMs = (UnityEngine.Time.timeAsDouble * 1000.0 * PlaybackSpeed);

            // ターゲット時間に追いつくまで、またはファイルの終端に達するまでフレームを読み込む
            while (_isInitialized && _isPlaying)
            {
                LogFrameData frameToProcess = _currentFrameDataBuffer;
                if (frameToProcess == null)
                {
                    // 次のフレームデータを非同期で読み込む
                    bool _;
                    (_, frameToProcess) = await ReadNextFrameUniTaskAsync().SuppressCancellationThrow(); // キャンセル例外を抑制

                    if (frameToProcess == null)
                    {
                        UnityEngine.Debug.Log("ログファイルの終端に達しました。");
                        _isPlaying = false; // ファイル終端で再生を停止
                        _currentFrameDataBuffer = null;
                        break;
                    }

                    // 最初のフレームタイムスタンプを記録
                    if (!_firstTimestampMillis.HasValue)
                    {
                        _firstTimestampMillis = frameToProcess.TimestampMs;
                    }
                }

                // 最初のフレームからの相対タイムスタンプを計算
                // この `relativeTimestampMs` は、ログの記録時からの経過時間 (ミリ秒)
                double relativeTimestampMs = frameToProcess.TimestampMs - _firstTimestampMillis.Value;

                // Unityの時間とログの相対時間を比較
                if (relativeTimestampMs <= currentUnityTimeMs)
                {
                    // 現在のUnity時間に間に合うフレームなので処理する
                    // イベント発火: HolisticLandmarks と float 型の相対タイムスタンプ (ミリ秒)
                    OnLandmarksReceived?.Invoke(frameToProcess.HolisticLandmarks, (float)relativeTimestampMs);
                    _currentFrameDataBuffer = null; // このフレームは処理されたのでバッファをクリア
                    // UnityEngine.Debug.Log($"フレーム {frameToProcess.FrameNumber} を処理しました ({frameToProcess.TimestampMs}ms, 相対: {(float)relativeTimestampMs}ms)");
                }
                else
                {
                    // このフレームはUnityの現在の時間より進んでいるので、次回のためにバッファして待つ
                    _currentFrameDataBuffer = frameToProcess;
                    break; // 次の Update サイクルでこのフレームを待つ
                }
            }
        }

        /// <summary>
        /// 次のログフレームデータを非同期で読み込む (UniTask 版)。ファイルの終端に達したら null を返す。
        /// </summary>
        private async UniTask<LogFrameData> ReadNextFrameUniTaskAsync()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("リーダーが初期化されていません。Initialize を先に呼び出してください。");
            }

            // UniTask.Yield() を挟むことで、次のフレーム読み込みを待機中にUnityがフリーズしないようにする
            // 非常に大きなファイルを読み込む場合などに有効。
            await UniTask.Yield(PlayerLoopTiming.Update); // または FixedUpdate, PostLateUpdate など適切に

            // 次のフレームデータの位置から読み込みを再開する
            _fileStream.Seek(_nextFrameOffset, SeekOrigin.Begin);

            if (_fileStream.Position + 4 > _fileStream.Length)
            {
                // ファイルの終端に達したか、次の長さプレフィックスが読み取れない
                return null;
            }

            byte[] lengthBytes = _binaryReader.ReadBytes(4);
            if (lengthBytes.Length < 4) return null; // 読み込めなかった場合
            int frameLength = BitConverter.ToInt32(lengthBytes, 0);

            if (_fileStream.Position + frameLength > _fileStream.Length)
            {
                UnityEngine.Debug.LogError($"警告: フレームデータが不完全です。期待される長さ: {frameLength}, 残りバイト: {_fileStream.Length - _fileStream.Position}");
                return null;
            }

            byte[] frameBytes = _binaryReader.ReadBytes(frameLength);

            LogFrameData frameData = _logFrameDataParser.ParseFrom(frameBytes);

            // 次の読み込みのためにストリーム位置を更新する
            _nextFrameOffset = _fileStream.Position;

            return frameData;
        }

        /// <summary>
        /// リソースを解放する。
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
            UnityEngine.Debug.Log("LandmarkProtoBinaryLogReader が Dispose されました。");
        }
    }
}