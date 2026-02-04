using UnityEngine;
using UniVRM10;
using VRM;

using mpBlendShapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;


namespace VioletSolver.Samples
{
    public class Vrm10AvatarAnimationController : MonoBehaviour
    {
        [Header("Animation Dependencies")]
        [SerializeField] Animator _animator;
        [SerializeField] Vrm10Instance _vrm10Instance;
        [SerializeField] GameObject _ikRigRoot;
        [SerializeField] LandmarkProviderBase _landmarkProvider;
        [SerializeField] Transform _offset;

        [Header("Animation Settings")]
        [SerializeField] bool _isAnimationEnabled = true;
        [SerializeField] bool _enableLeg = false;
        [SerializeField] bool _isPerfectSyncEnabled = false;
        [SerializeField] bool _isIkEnabled = true;

        [Header("Calibration Settings")]
        [SerializeField] bool _isCalibrationEnabled = true;
        [SerializeField] int _calibrationSamples = 30;

        [Header("Misc")]
        [SerializeField] SkinnedMeshRenderer _faceAssetObject;
        [SerializeField] SkinnedMeshRenderer _bodyAssetObject;
        [SerializeField] SkinnedMeshRenderer _hairAssetObject;

        ArmLengthCalibrator _armLengthCalibrator;
        protected LandmarkHandler _landmarkHandler;
        AvatarAnimator _avatarAnimator;
        AssetsPositionAdjuster _assetsPositionSynchronizer;
        PoseInterpolator _poseInterpolator;
        BlendshapeInterpolator<BlendShapePreset> _vrmExpressionInterpolator;
        BlendshapeInterpolator<mpBlendShapes> _perfectSyncExpressionInterpolator;

        protected virtual void Awake()
        {
            if (_animator is null
                || _vrm10Instance is null
                || _ikRigRoot is null
                || _landmarkProvider is null)
            {
                Debug.LogError("All animation dependencies must be assigned in the Inspector.", this);
                enabled = false;
                return;
            }

            _assetsPositionSynchronizer = new AssetsPositionAdjuster
            {
                Animator = _animator,
                Face = _faceAssetObject,
                Body = _bodyAssetObject,
                Hair = _hairAssetObject,
            };
            _assetsPositionSynchronizer.Adjust();

            _landmarkHandler = new LandmarkHandler(_landmarkProvider);
            var poseHandler = new Pose.PoseHandler();

            IBlendshapeSolver blendshapeSolver = _isPerfectSyncEnabled
                ? new PerfectSyncBlendshapeSolver()
                : new StandardBlendshapeSolver();
            IFaceApplier faceApplier = new Vrm10FaceApplier(_vrm10Instance.Runtime.Expression);

            _avatarAnimator = new AvatarAnimator(
                _ikRigRoot,
                _animator,
                _landmarkHandler,
                poseHandler,
                blendshapeSolver,
                faceApplier
            );
            _poseInterpolator = new PoseInterpolator();
            _vrmExpressionInterpolator = new BlendshapeInterpolator<BlendShapePreset>();
            _perfectSyncExpressionInterpolator = new BlendshapeInterpolator<HumanLandmarks.Blendshapes.Types.BlendshapesIndex>();

            // Initialize calibrator and subscribe to landmark updates.
            if (_isCalibrationEnabled)
            {
                _armLengthCalibrator = new ArmLengthCalibrator(_animator, _calibrationSamples);
                _landmarkProvider.OnLandmarksReceived += OnLandmarksReceivedForCalibration;
            }
        }

        void OnLandmarksReceivedForCalibration(HumanLandmarks.HolisticLandmarks landmarks, float time)
        {
            if (_armLengthCalibrator == null || _armLengthCalibrator.IsCalibrated)
                return;

            // Use Kinect pose landmarks for calibration.
            if (landmarks.KinectPoseLandmarks == null)
                return;

            var kinectPose = _landmarkHandler.Landmarks.KinectPose;
            _armLengthCalibrator.Update(kinectPose);

            if (_armLengthCalibrator.IsCalibrated)
                Debug.Log($"Calibration completed. Scale: {_armLengthCalibrator.Scale:F3}");
            else
                Debug.Log($"Calibration progress: {_armLengthCalibrator.Progress:P0}");
        }

        protected virtual void Update()
        {
            if (_isAnimationEnabled)
            {
                var animationData = _avatarAnimator.CalculateAnimationData(_isIkEnabled);

                // Apply calibration scale to position data.
                var scale = 1f;
                if (_armLengthCalibrator is { IsCalibrated: true })
                {
                    scale = _armLengthCalibrator.Scale;
                    animationData.PoseData.ScalePositions(scale);
                }

                animationData.PoseData = _poseInterpolator.UpdateAndInterpolate(animationData.PoseData);
                if (animationData.PerfectSyncBlendshapes is not null)
                    animationData.PerfectSyncBlendshapes = _perfectSyncExpressionInterpolator.UpdateAndInterpolate(animationData.PerfectSyncBlendshapes, animationData.PoseData.time);
                else if (animationData.VrmBlendshapes is not null)
                    animationData.VrmBlendshapes = _vrmExpressionInterpolator.UpdateAndInterpolate(animationData.VrmBlendshapes, animationData.PoseData.time);

                _avatarAnimator.ApplyAnimationData(animationData, _isIkEnabled, _enableLeg, _offset);

                OnPostUpdate(scale);
            }
        }

        protected virtual void OnPostUpdate(float scale) { }
    }
}
