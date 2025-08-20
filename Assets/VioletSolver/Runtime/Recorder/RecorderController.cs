using UnityEngine;
using VRM;

namespace VioletSolver.Recorder
{
    public class RecorderController : MonoBehaviour
    {
        // Header information for the log file
        [Header("Recording Avatar")]
        [SerializeField] Animator _avatarAnimator;
        [SerializeField] VRMBlendShapeProxy _blendShapeProxy;
        [SerializeField] string _avatarName;
        [SerializeField] string _avatarVersion;

        AvatarMotionRecorder _avatarMotionRecorder;

        public void StartRecording() => _avatarMotionRecorder?.StartRecording();
        public void PauseRecording() => _avatarMotionRecorder?.PauseRecording();
        public void StopRecording() => _avatarMotionRecorder?.StopRecording();

        void Awake()
        {
            _avatarMotionRecorder = new AvatarMotionRecorder(_avatarAnimator, _blendShapeProxy, _avatarName, _avatarVersion);
        }

        void Start()
        {
            bool isInitialized = _avatarMotionRecorder.Initialzie("avatar_motion_log.pb");

            if(!isInitialized)
            {
                Debug.LogError("Failed to initialize AvatarMotionRecorder. Please check the log file path and permissions.");
                enabled = false; // Disable this component if initialization fails.
            }
        }

        void Update()
        {
            if (enabled)
                _avatarMotionRecorder.Update();
        }

        void OnDestroy()
        {
            if (_avatarMotionRecorder is not null)
            {
                _avatarMotionRecorder.Dispose();
                _avatarMotionRecorder = null;
            }
        }
    }
}
