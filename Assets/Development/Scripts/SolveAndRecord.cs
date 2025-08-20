using UnityEngine;
using VioletSolver.LandmarkProviders;
using VioletSolver.Recorder;

namespace VioletSolver.Development
{
    public class SolveAndRecord : MonoBehaviour
    {
        [SerializeField] LandmarkPlaybackManager _playbackManager;
        [SerializeField] RecorderController _motionRecorder;
        bool IsPlayingLog => _playbackManager.IsPlayingLog;

        void Start()
        {
            if (_playbackManager == null)
            {
                Debug.LogError("Playback Manager is not assigned in the inspector.");
                enabled = false;
                return;
            }
            if(_motionRecorder == null)
            {
                Debug.LogError("Avatar Motion Recorder is not assigned in the inspector.");
                enabled = false;
                return;
            }

            _playbackManager.OnReachedToEnd += HandleOnReachedToEnd;
        }

        void HandleOnReachedToEnd()
        {
            _motionRecorder.StopRecording();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (IsPlayingLog)
                {
                    _playbackManager.PausePlayback();
                    _motionRecorder.PauseRecording();
                }
                else
                {
                    _motionRecorder.StartRecording();
                    _playbackManager.StartPlayback();
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                _playbackManager.ResetPlayback();
                _motionRecorder.StopRecording();
            }
        }

        void OnDestroy()
        {
            if (_playbackManager != null)
            {
                _playbackManager.OnReachedToEnd -= HandleOnReachedToEnd;
            }
        }
    }
}
