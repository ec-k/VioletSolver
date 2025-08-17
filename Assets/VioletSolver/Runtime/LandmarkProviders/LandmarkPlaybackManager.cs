using System;
using UnityEngine;

// 生成されたProtobuf C#クラスのusing (プロジェクトの構造によって異なる場合がある)
using HumanLandmarks;
using HumanLandmarks.Log;

namespace VioletSolver.LandmarkProviders
{
    /// <summary>
    /// アバターの姿勢再生を管理するUnityコンポーネント。
    /// ILandmarkProvider インターフェースを実装し、ILandmarkLogReader を使用してログデータを読み込む。
    /// </summary>
    public class LandmarkPlaybackManager : MonoBehaviour, ILandmarkProvider
    {
        // ILandmarkProvider インターフェースのイベント実装
        public event Action<HumanLandmarks.HolisticLandmarks, float> OnLandmarksReceived;

        [Tooltip("再生するログファイルのパス。アセットパスまたは絶対パス。")]
        public string logFilePath;

        [Tooltip("ログの再生速度 (1.0でリアルタイム)。")]
        [Range(0.1f, 10.0f)]
        public float playbackSpeed = 1.0f;

        // ILandmarkLogReader のインスタンスを保持
        private ILandmarkLogReader _logFileReader;

        // ILandmarkProvider インターフェースのプロパティ実装
        public bool IsPlayingLog => _logFileReader?.IsPlaying ?? false;

        public float CurrentPlaybackSpeed
        {
            get => _logFileReader?.PlaybackSpeed ?? 0f;
            set
            {
                playbackSpeed = value; // インスペクタの値を更新
                if (_logFileReader != null)
                {
                    _logFileReader.PlaybackSpeed = value;
                }
            }
        }

        // ILandmarkProvider インターフェースに追加された LogHeader プロパティ
        // _logFileReader が初期化されていればその LogHeader を返す
        public LogHeader LogHeader => _logFileReader?.LogHeader;


        // 外部から制御するためのパブリックメソッド (ILandmarkProvider インターフェースの一部)
        public void Play() => _logFileReader?.StartPlayback();
        public void Pause() => _logFileReader?.StopPlayback();
        public void Rewind() => _logFileReader?.ResetPlayback();

        // Unityのライフサイクルメソッド
        void Awake()
        {
            // ここで具体的な実装のログリーダーをインスタンス化する
            // コンストラクタでパスを渡す必要はなくなったので、Initializeメソッドで設定
            _logFileReader = new LandmarkProtoBinaryLogReader(); // パスを渡さないコンストラクタを使用

            // _logFileReaderからのイベントをフック
            _logFileReader.OnLandmarksReceived += OnLogLandmarksReceived;
        }

        void Start()
        {
            OnLandmarksReceived += LoggingForDebug;
            // インスペクタで設定されたパスでPOCOを初期化
            // Initializeメソッドは bool を返すので、その結果を確認する
            bool isInitialized = _logFileReader.Initialize(logFilePath);

            if (isInitialized)
            {
                _logFileReader.PlaybackSpeed = playbackSpeed; // Initialize後にPlaybackSpeedを設定
                // シーン開始時に自動で再生を開始したい場合
                _logFileReader.StartPlayback();
            }
            else
            {
                Debug.LogError($"ログファイルリーダーの初期化に失敗しました。パス: {logFilePath}");
                // 初期化失敗時はこのコンポーネントを無効化して、Updateが呼ばれないようにする
                enabled = false;
            }
        }

        void LoggingForDebug(HumanLandmarks.HolisticLandmarks result, float timeMs)
        {
            UnityEngine.Debug.Log($"Received time: {timeMs}");
        }

        void Update()
        {
            // コンポーネントが無効化されている場合は何もしない
            if (!enabled) return;

            // _logFileReaderのUpdateメソッドを毎フレーム呼び出す
            _logFileReader?.Update(Time.deltaTime);

            // ここでUIからの操作などを検知し、_logFileReader.StartPlayback(), StopPlayback(), ResetPlayback() を呼び出す
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
            if (Input.GetKeyDown(KeyCode.R)) // Rキーでリセット
            {
                _logFileReader?.ResetPlayback();
            }
        }

        // _logFileReaderから受け取ったイベントをそのまま外部に発火
        private void OnLogLandmarksReceived(HumanLandmarks.HolisticLandmarks landmarks, float relativeTime)
        {
            OnLandmarksReceived?.Invoke(landmarks, relativeTime);
        }

        // リソースを解放
        void OnDestroy()
        {
            if (_logFileReader is not null)
            {
                // イベントの購読を解除
                _logFileReader.OnLandmarksReceived -= OnLogLandmarksReceived;
                // リソースを解放
                _logFileReader.Dispose();
                _logFileReader = null;
            }
        }
    }
}