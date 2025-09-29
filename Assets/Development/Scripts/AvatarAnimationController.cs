using UnityEngine;
using VRM;
using HumanoidPoseConnector;

using VioletSolver.Pose;
using VioletSolver.LandmarkProviders;

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

        [Header("Animation Settings")]
        [SerializeField] bool _isAnimationEnabled = true;
        [SerializeField] bool _animateLeg = false;
        [SerializeField] bool _isPerfectSyncEnabled = false;
        [SerializeField] bool _isIkEnabled = true;

        [Header("External Input Settings")]
        [SerializeField] bool _isOverrideEnabled = false;

        [Header("Misc")]
        [SerializeField]bool _isRealtime = true;
        [SerializeField] SkinnedMeshRenderer _faceAssetObject;
        [SerializeField] SkinnedMeshRenderer _bodyAssetObject;
        [SerializeField] SkinnedMeshRenderer _hairAssetObject;

        ILandmarkProvider _landmarkProvider;
        LandmarkHandler _landmarkHandler;
        AvatarAnimator _avatarAnimator;
        AssetsPositionAdjuster _assetsPositionSynchronizer;

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

            if (_landmarkVisualizer is not null)
                _landmarkVisualizer.Initialize(_landmarkHandler);
            else
                Debug.LogWarning("LandmarkVisualizer is not assigned. Landmark visualization will not work.", this);
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

                _avatarAnimator.ApplyAnimationData(animationData, _isIkEnabled, false);
            }
        }
    }
}
