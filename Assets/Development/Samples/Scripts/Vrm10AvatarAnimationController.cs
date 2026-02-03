using UnityEngine;
using UniVRM10;
using HumanoidPoseConnector;

using VioletSolver.Pose;
using VRM;

namespace VioletSolver.Samples
{
    public class Vrm10AvatarAnimationController : MonoBehaviour
    {
        [Header("Animation Dependencies")]
        [SerializeField] Animator _animator;
        [SerializeField] Vrm10Instance _vrm10Instance;
        [SerializeField] GameObject _ikRigRoot;
        [SerializeField] LandmarkProviderBase _landmarkProvider;
        [SerializeField] HumanoidPoseReceiver _poseReceiver;
        [SerializeField] Transform _offset;

        [Header("Animation Settings")]
        [SerializeField] bool _isAnimationEnabled = true;
        [SerializeField] bool _animateLeg = false;
        [SerializeField] bool _isPerfectSyncEnabled = false;
        [SerializeField] bool _isIkEnabled = true;

        [Header("External Input Settings")]
        [SerializeField] bool _isOverrideEnabled = false;

        [Header("Misc")]
        [SerializeField] bool _isRealtime = true;
        [SerializeField] SkinnedMeshRenderer _faceAssetObject;
        [SerializeField] SkinnedMeshRenderer _bodyAssetObject;
        [SerializeField] SkinnedMeshRenderer _hairAssetObject;

        protected LandmarkHandler _landmarkHandler;
        AvatarAnimator _avatarAnimator;
        AssetsPositionAdjuster _assetsPositionSynchronizer;
        PoseInterpolator _poseInterpolator;
        BlendshapeInterpolator<BlendShapePreset> _vrmExpressionInterpolator;
        BlendshapeInterpolator<HumanLandmarks.Blendshapes.Types.BlendshapesIndex> _perfectSyncExpressionInterpolator;

        protected virtual void Awake()
        {
            if (_animator is null
                || _vrm10Instance is null
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

            IBlendshapeSolver blendshapeSolver = _isPerfectSyncEnabled
                ? new PerfectSyncBlendshapeSolver()
                : new StandardBlendshapeSolver();
            IFaceApplier faceApplier = new Vrm10FaceApplier(_vrm10Instance.Runtime.Expression);

            _avatarAnimator = new AvatarAnimator(
                _ikRigRoot,
                _animator,
                _landmarkHandler,
                blendshapeSolver,
                faceApplier
            );
            _poseInterpolator = new PoseInterpolator();
            _vrmExpressionInterpolator = new BlendshapeInterpolator<BlendShapePreset>();
            _perfectSyncExpressionInterpolator = new BlendshapeInterpolator<HumanLandmarks.Blendshapes.Types.BlendshapesIndex>();
        }

        protected virtual void Update()
        {
            if (_isAnimationEnabled)
            {
                var animationData = _avatarAnimator.CalculateAnimationData(_isIkEnabled);

                if (_isOverrideEnabled && _poseReceiver.IsAvailable)
                {
                    var externalPoseResults = _poseReceiver.PoseResults;
                    foreach (var bone in BodyPartsBones.Fingers)
                    {
                        if (externalPoseResults.TryGetValue(bone, out (Vector3 pos, Quaternion rot) boneTransform))
                            animationData.PoseData[bone] = boneTransform.rot;
                    }
                }

                animationData.PoseData = _poseInterpolator.UpdateAndInterpolate(animationData.PoseData);
                if (animationData.PerfectSyncBlendshapes is not null)
                    animationData.PerfectSyncBlendshapes = _perfectSyncExpressionInterpolator.UpdateAndInterpolate(animationData.PerfectSyncBlendshapes, animationData.PoseData.time);
                else if (animationData.VrmBlendshapes is not null)
                    animationData.VrmBlendshapes = _vrmExpressionInterpolator.UpdateAndInterpolate(animationData.VrmBlendshapes, animationData.PoseData.time);

                _avatarAnimator.ApplyAnimationData(animationData, _isIkEnabled, _animateLeg, _offset);

                OnPostUpdate(1f);
            }
        }

        protected virtual void OnPostUpdate(float scale) { }
    }
}
