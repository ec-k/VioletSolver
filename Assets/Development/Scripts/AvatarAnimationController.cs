using UnityEngine;
using VRM;
using HumanoidPoseConnector;

using VioletSolver.Pose;
using VioletSolver.LandmarkProviders;
using VioletSolver.Landmarks;

namespace VioletSolver.Development
    {
    public class AvatarAnimationController : MonoBehaviour
    {
        [Header("Animation Dependencies")]
        [SerializeField] Animator _animator;
        [SerializeField] VRMBlendShapeProxy _blendshapeProxy;
        [SerializeField] GameObject _ikRigRoot;
        [SerializeField] LandmarkReceiver _landmarkReceiver;
        [SerializeField] LandmarkPlaybackManager _landmarkPlaybackManager;
        [SerializeField] LandmarkVisualizer _landmarkVisualizer;
        [SerializeField] HumanoidPoseReceiver _poseReceiver;
        [SerializeField] Transform _offset;

        [Header("Animation Settings")]
        [SerializeField] bool _isAnimationEnabled = true;
        [SerializeField] bool _enableLeg = false;
        [SerializeField] bool _isPerfectSyncEnabled = false;
        [SerializeField] bool _isIkEnabled = true;

        [Header("External Input Settings")]
        [SerializeField] bool _isOverrideEnabled = false;

        [Header("Calibration Settings")]
        [SerializeField] bool _isCalibrationEnabled = true;
        [SerializeField] int _calibrationSamples = 30;

        [Header("Misc")]
        [SerializeField]bool _isRealtime = true;
        [SerializeField] SkinnedMeshRenderer _faceAssetObject;
        [SerializeField] SkinnedMeshRenderer _bodyAssetObject;
        [SerializeField] SkinnedMeshRenderer _hairAssetObject;

        ILandmarkProvider _landmarkProvider;
        ArmLengthCalibrator _armLengthCalibrator;
        LandmarkHandler _landmarkHandler;
        AvatarAnimator _avatarAnimator;
        AssetsPositionAdjuster _assetsPositionSynchronizer;
        PoseInterpolator _poseInterpolator;
        BlendshapeInterpolator<VRM.BlendShapePreset> _vrmBlendshapeInterpolator;
        BlendshapeInterpolator<HumanLandmarks.Blendshapes.Types.BlendshapesIndex> _perfectSyncBlendshapeInterpolator;

        void Awake()
        {
            _landmarkProvider = _isRealtime? _landmarkReceiver : _landmarkPlaybackManager;

            if (_animator is null
                || _blendshapeProxy is null
                || _ikRigRoot is null
                || _landmarkProvider is null
                || _poseReceiver is null)
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
            _avatarAnimator = new AvatarAnimator(
                _ikRigRoot,
                _animator,
                _blendshapeProxy,
                _landmarkHandler,
                _isPerfectSyncEnabled
            );
            _poseInterpolator = new PoseInterpolator();
            _vrmBlendshapeInterpolator = new BlendshapeInterpolator<VRM.BlendShapePreset>();
            _perfectSyncBlendshapeInterpolator = new BlendshapeInterpolator<HumanLandmarks.Blendshapes.Types.BlendshapesIndex>();

            if (_landmarkVisualizer is not null)
                _landmarkVisualizer.Initialize(_landmarkHandler);
            else
                Debug.LogWarning("LandmarkVisualizer is not assigned. Landmark visualization will not work.", this);

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

        void Update()
        {
            if (_isAnimationEnabled)
            {
                var animationData = _avatarAnimator.CalculateAnimationData(_isIkEnabled);

                if (_isOverrideEnabled && _poseReceiver.IsAvailable)
                {
                    var externalPoseResults = _poseReceiver.PoseResults;
                    foreach(var bone in BodyPartsBones.Fingers)
                    {
                        if (externalPoseResults.TryGetValue(bone, out (Vector3 pos, Quaternion rot) boneTransform))
                            animationData.PoseData[bone] = boneTransform.rot;
                    }
                }

                // Apply calibration scale to position data.
                var scale = 1f;
                if (_armLengthCalibrator is { IsCalibrated: true })
                {
                    scale = _armLengthCalibrator.Scale;
                    animationData.PoseData.ScalePositions(scale);
                }

                animationData.PoseData = _poseInterpolator.UpdateAndInterpolate(animationData.PoseData);
                if (animationData.PerfectSyncBlendshapes != null && _isPerfectSyncEnabled)
                    animationData.PerfectSyncBlendshapes = _perfectSyncBlendshapeInterpolator.UpdateAndInterpolate(animationData.PerfectSyncBlendshapes, animationData.PoseData.time);
                else if (animationData.VrmBlendshapes != null)
                    animationData.VrmBlendshapes = _vrmBlendshapeInterpolator.UpdateAndInterpolate(animationData.VrmBlendshapes, animationData.PoseData.time);

                _avatarAnimator.ApplyAnimationData(animationData, _isIkEnabled, _enableLeg, _offset);

                // Update landmark visualization with the same scale applied to the avatar.
                if (_landmarkVisualizer is not null)
                {
                    _landmarkVisualizer.SetScale(scale);
                    _landmarkVisualizer.UpdateVisualization();
                }
            }
        }
    }
}
