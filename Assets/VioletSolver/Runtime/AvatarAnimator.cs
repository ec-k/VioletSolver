using Cysharp.Threading.Tasks;
using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UniVRM10;
using VioletSolver.Pose;
using VioletSolver.Solver;
using VRM;
using mpBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

// This is avatar animating component which does
//  1. gets landmarks and filters landmarks (in _landmarkHandler)
//  2. solve landmarks to avatar pose
//  3. filters pose and apply pose to avatar (in _avatarPoseHandler)
namespace VioletSolver 
{
    public class AvatarAnimator
    {
        public Animator Animator { get; set; }
        public VRMBlendShapeProxy BlendShapeProxy { get; set; }
        public Vrm10RuntimeExpression Expression { get; set; }
        readonly GameObject _ikRigRoot;
        readonly bool _isPerfectSyncEnabled = false;

        readonly LandmarkHandler _landmarkHandler;
        readonly PoseHandler _avatarPoseHandler;
        readonly AvatarBonePositions _restBonePositions;

        readonly ArmIK _leftArmIk;
        readonly ArmIK _rightArmIk;
        // IK Targets
        readonly Transform _leftShoulderTarget;
        readonly Transform _leftElbowTarget;
        readonly Transform _leftHandTarget;
        readonly Transform _rightShoulderTarget;
        readonly Transform _rightElbowTarget;
        readonly Transform _rightHandTarget;

        bool _isVrm10;

        public AvatarAnimator(
            GameObject ikRigRoot,
            Animator animator,
            VRMBlendShapeProxy blendShapeProxy,
            LandmarkHandler landmarkHandler,
            bool isPerfectSyncEnabled)
        {
            _landmarkHandler = landmarkHandler;
            _avatarPoseHandler = new();

            _ikRigRoot = ikRigRoot;
            Animator = animator;
            BlendShapeProxy = blendShapeProxy;
            _isPerfectSyncEnabled = isPerfectSyncEnabled;

            _restBonePositions = AvatarBonePositionsInitializer.CreateFromAnimator(animator);

            ArmIKSetup.Initialize(
                _ikRigRoot,
                Animator,
                out _leftShoulderTarget,
                out _leftElbowTarget,
                out _leftHandTarget,
                out _rightShoulderTarget,
                out _rightElbowTarget,
                out _rightHandTarget,
                out _leftArmIk,
                out _rightArmIk);
            
            _isVrm10 = false;
        }

        public AvatarAnimator(
            GameObject ikRigRoot,
            Animator animator,
            Vrm10RuntimeExpression expression,
            LandmarkHandler landmarkHandler,
            bool isPerfectSyncEnabled)
        {
            _landmarkHandler = landmarkHandler;
            _avatarPoseHandler = new();

            _ikRigRoot = ikRigRoot;
            Animator = animator;
            Expression = expression;
            _isPerfectSyncEnabled = isPerfectSyncEnabled;

            _restBonePositions = AvatarBonePositionsInitializer.CreateFromAnimator(animator);

            ArmIKSetup.Initialize(
                _ikRigRoot,
                Animator,
                out _leftShoulderTarget,
                out _leftElbowTarget,
                out _leftHandTarget,
                out _rightShoulderTarget,
                out _rightElbowTarget,
                out _rightHandTarget,
                out _leftArmIk,
                out _rightArmIk);

            _isVrm10 = true;
        }

        public AnimationResultData CalculateAnimationData(bool isIkEnabled)
        {
            UpdatePose(isIkEnabled);

            Dictionary<BlendShapePreset, float> vrmBs = null;
            Dictionary<mpBlendshapes, float> mpBs = null;
            if (_isPerfectSyncEnabled)
            {
                UpdateBlendshapesPerfectly();
                mpBs = _avatarPoseHandler.PerfectSyncWeights;
            }
            else
            {
                UpdateBlendshapes();
                vrmBs = _avatarPoseHandler.BlendshapeWeights;
            }

            return new AnimationResultData
            {
                PoseData = _avatarPoseHandler.PoseData,
                VrmBlendshapes = vrmBs,
                PerfectSyncBlendshapes = mpBs
            };
        }

        public void ApplyAnimationData(AnimationResultData data, bool isIkEnabled, bool animateLeg, Transform? offset = null)
        {
            _leftArmIk.enabled = isIkEnabled;
            _rightArmIk.enabled = isIkEnabled;

            AnimateAvatar(Animator, data.PoseData, isIkEnabled, animateLeg, offset);

            if (_isPerfectSyncEnabled)
            {
                if (data.PerfectSyncBlendshapes is null) return;

                if (_isVrm10)
                    AnimateFace(Expression, data.PerfectSyncBlendshapes);
                else
                    AnimateFace(BlendShapeProxy, data.PerfectSyncBlendshapes);
            }
            else
            {
                if (data.VrmBlendshapes is null) return;

                if(_isVrm10)
                    AnimateFace(Expression, data.VrmBlendshapes);
                else
                    AnimateFace(BlendShapeProxy, data.VrmBlendshapes);
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>Whether updating landmarks is processed properly or not.</returns>
        void UpdatePose(bool isIkEnabled)
        {
            var isKinectPose = _landmarkHandler.IsKinectPose;

            var landmarks = _landmarkHandler.Landmarks;
            var pose = HolisticSolver.Solve(landmarks, _restBonePositions, isIkEnabled, isKinectPose);
            pose.time = isKinectPose ? landmarks.KinectPose.Time : landmarks.MediaPipePose.Time;
            _avatarPoseHandler.Update(pose);
        }
        void UpdateBlendshapes()
        {
            var mpBlendshapes = _landmarkHandler.MpBlendshapes;
            if (mpBlendshapes == null ||
                mpBlendshapes.Count <= 0)
                return;
            var (blendshapes, leftEye, rightEye) = HolisticSolver.Solve(mpBlendshapes);
            _avatarPoseHandler.Update(blendshapes);
            _avatarPoseHandler.Update(HumanBodyBones.LeftEye, leftEye);
            _avatarPoseHandler.Update(HumanBodyBones.RightEye, rightEye);
        }
        void UpdateBlendshapesPerfectly()
        {
            var mpBlendshapes = _landmarkHandler.MpBlendshapes;
            if (mpBlendshapes == null ||
                mpBlendshapes.Count <= 0)
                return;
            var (blendshapes, leftEye, rightEye) = HolisticSolver.SolvePerfectly(mpBlendshapes);
            _avatarPoseHandler.Update(blendshapes);
            _avatarPoseHandler.Update(HumanBodyBones.LeftEye, leftEye);
            _avatarPoseHandler.Update(HumanBodyBones.RightEye, rightEye);
        }

        void AnimateAvatar(Animator animator, AvatarPoseData pose, bool isIkEnabled, bool animateLeg, Transform? offset = null)
        {
            if (offset is not null)
                animator.GetBoneTransform(HumanBodyBones.Hips).position = offset.rotation * pose.HipsPosition + offset.position;
            else
                animator.GetBoneTransform(HumanBodyBones.Hips).position = pose.HipsPosition;

            if (!animateLeg)
                foreach (var bone in BodyPartsBones.Legs)
                    pose[bone] = Quaternion.identity;

            // Spines
            foreach (var bone in BodyPartsBones.Spines)
                ApplyGlobal(animator, pose, bone, offset);

            // Legs
            foreach(var bone in BodyPartsBones.Legs)
                ApplyGlobal(animator, pose, bone, offset);

            // Arms.
            if (isIkEnabled)
                ApplyIkTarget(pose, offset);
            else
                foreach (var bone in BodyPartsBones.Arms)
                    ApplyGlobal(animator, pose, bone, offset);

            // Fingers
            foreach (var bone in BodyPartsBones.Fingers)
                ApplyLocal(animator, pose, bone);

            // Eyes
            foreach (var bone in BodyPartsBones.Eyes)
                ApplyLocal(animator, pose, bone);
        }

        void ApplyIkTarget(AvatarPoseData pose, Transform? offset = null)
        {
            if (offset is not null)
            {
                _ikRigRoot.transform.position = offset.position;
                _ikRigRoot.transform.rotation = offset.rotation;
            }

            _leftShoulderTarget.localPosition = pose.LeftShoulderPosition;
            _leftElbowTarget.localPosition = pose.LeftElbowPosition;
            _leftHandTarget.localPosition = pose.LeftHandPosition;
            _leftHandTarget.localRotation = pose.LeftHand;

            _rightShoulderTarget.localPosition = pose.RightShoulderPosition;
            _rightElbowTarget.localPosition = pose.RightElbowPosition;
            _rightHandTarget.localPosition = pose.RightHandPosition;
            _rightHandTarget.localRotation = pose.RightHand;
        }

        void ApplyLocal(Animator animator, AvatarPoseData pose, HumanBodyBones boneName)
            => animator.GetBoneTransform(boneName).localRotation = pose[boneName];

        void ApplyGlobal(Animator animator, AvatarPoseData pose, HumanBodyBones boneName, Transform? offset = null)
        {
            if (offset is not null)
                animator.GetBoneTransform(boneName).rotation = offset.rotation * pose[boneName];
            else
                animator.GetBoneTransform(boneName).rotation = pose[boneName];
        }

        void AnimateFace(VRMBlendShapeProxy proxy, Dictionary<BlendShapePreset, float> blendshapes)
        {
            var bs = new Dictionary<BlendShapeKey, float>();

            var tmpArray = Enum.GetValues(typeof(BlendShapePreset));
            foreach (var value in tmpArray)
            {
                var blendshapeIndex = (BlendShapePreset)value;
                if (blendshapes.TryGetValue(blendshapeIndex, out var blendshape))
                    bs[BlendShapeKey.CreateFromPreset(blendshapeIndex)] = blendshape;
            }

            proxy.SetValues(bs);
        }        
        
        void AnimateFace(VRMBlendShapeProxy proxy, Dictionary<mpBlendshapes, float> blendshapes)
        {
            var bs = new Dictionary<BlendShapeKey, float>();

            var tmpArray = Enum.GetValues(typeof(mpBlendshapes));
            foreach (var value in tmpArray)
            {
                var blendshapeIndex = (mpBlendshapes)value;
                if (blendshapes.TryGetValue(blendshapeIndex, out var blendshape))
                    bs[BlendShapeKey.CreateUnknown(blendshapeIndex.ToString())] = blendshape;
            }

            proxy.SetValues(bs);
        }

        void AnimateFace(Vrm10RuntimeExpression expression, Dictionary<BlendShapePreset, float> blendshapes)
        {
            var expressionDicrionary = Enumerable.Range(0, Enum.GetValues(typeof(BlendShapePreset)).Length)
                .Select(i =>
                {
                    var vrm0xKey = (BlendShapePreset)i;

                    // NOTE: This process can NOT handles new expression "surprised".
                    var vrm10Key = vrm0xKey switch
                    {
                        BlendShapePreset.Joy => ExpressionPreset.happy,
                        BlendShapePreset.Angry => ExpressionPreset.angry,
                        BlendShapePreset.Sorrow => ExpressionPreset.sad,
                        BlendShapePreset.Fun => ExpressionPreset.relaxed,
                        BlendShapePreset.A => ExpressionPreset.aa,
                        BlendShapePreset.I => ExpressionPreset.ih,
                        BlendShapePreset.U => ExpressionPreset.ou,
                        BlendShapePreset.E => ExpressionPreset.ee,
                        BlendShapePreset.O => ExpressionPreset.oh,
                        _ => ExpressionPreset.custom
                    };

                    return new KeyValuePair<ExpressionKey, float>
                    (
                        ExpressionKey.CreateFromPreset(vrm10Key),
                        blendshapes[(BlendShapePreset)i]
                    );
                })
                .ToDictionary(
                    item => item.Key,
                    item => item.Value
                );

            foreach (var kvp in expressionDicrionary)
                expression.SetWeight(kvp.Key, kvp.Value);
        }

        void AnimateFace(Vrm10RuntimeExpression expression, Dictionary<mpBlendshapes, float> blendshapes)
        {
            var expressionDicrionary = Enumerable.Range(0, (int)mpBlendshapes.Length)
                .Select(i => (mpBlendshapes)i)
                .Where(mpKey => blendshapes.ContainsKey(mpKey))
                .ToDictionary(
                    mpKey => ExpressionKey.CreateCustom(Enum.GetName(typeof(mpBlendshapes), mpKey)),
                    mpKey => blendshapes[mpKey]
                );

            foreach (var kvp in  expressionDicrionary)
                expression.SetWeight(kvp.Key, kvp.Value);
        }
    }
}
