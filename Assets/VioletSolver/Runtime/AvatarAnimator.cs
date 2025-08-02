using System;
using System.Collections.Generic;
using UnityEngine;
using VRM;
using RootMotion.FinalIK;

using VioletSolver.Pose;
using VioletSolver.Solver;
using mpBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

// This is avatar animating component which does
//  1. gets landmarks and filters landmarks (in _landmarkHandler)
//  2. solve landmarks to avatar pose
//  3. filters pose and apply pose to avatar (in _avatarPoseHandler)
namespace VioletSolver 
{
    public class AvatarAnimator
    {
        readonly GameObject _ikRigRoot;
        readonly Animator _animator;
        readonly VRMBlendShapeProxy _blendshapeProxy;
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
            _animator = animator;
            _blendshapeProxy = blendShapeProxy;
            _isPerfectSyncEnabled = isPerfectSyncEnabled;

            _restBonePositions = AvatarBonePositionsInitializer.CreateFromAnimator(animator);

            ArmIKSetup.Initialize(
                _ikRigRoot,
                _animator,
                out _leftShoulderTarget,
                out _leftElbowTarget,
                out _leftHandTarget,
                out _rightShoulderTarget,
                out _rightElbowTarget,
                out _rightHandTarget,
                out _leftArmIk,
                out _rightArmIk);
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

        public void ApplyAnimationData(AnimationResultData data, bool isIkEnabled, bool animateLeg)
        {
            _leftArmIk.enabled = isIkEnabled;
            _rightArmIk.enabled = isIkEnabled;

            AnimateAvatar(_animator, data.PoseData, isIkEnabled, animateLeg);

            if (_isPerfectSyncEnabled)
            {
                if (data.PerfectSyncBlendshapes is not null)
                    AnimateFace(_blendshapeProxy, data.PerfectSyncBlendshapes);
            }
            else
            {
                if (data.VrmBlendshapes is not null)
                    AnimateFace(_blendshapeProxy, data.VrmBlendshapes);
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>Whether updating landmarks is processed properly or not.</returns>
        void UpdatePose(bool isIkEnabled)
        {
            var landmarks = _landmarkHandler.Landmarks;
            var pose = HolisticSolver.Solve(landmarks, _restBonePositions, isIkEnabled);
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

        void AnimateAvatar(Animator animator, AvatarPoseData pose, bool isIkEnabled, bool animateLeg)
        {
            animator.GetBoneTransform(HumanBodyBones.Hips).position = pose.HipsPosition;

            // Spines
            foreach (var bone in BodyPartsBones.Spines)
                ApplyGlobal(animator, pose, bone);

            // Legs
            if (animateLeg)
                foreach(var bone in BodyPartsBones.Legs)
                    ApplyGlobal(animator, pose, bone);

            // Arms.
            if (isIkEnabled)
                ApplyIkTarget(pose);
            else
                foreach (var bone in BodyPartsBones.Arms)
                    ApplyGlobal(animator, pose, bone);

            // Fingers
            foreach (var bone in BodyPartsBones.Fingers)
                ApplyLocal(animator, pose, bone);

            // Eyes
            foreach (var bone in BodyPartsBones.Eyes)
                ApplyLocal(animator, pose, bone);
        }

        void ApplyIkTarget(AvatarPoseData pose)
        {
            _leftShoulderTarget.position = pose.LeftShoulderPosition;
            _leftElbowTarget.position = pose.LeftElbowPosition;
            _leftHandTarget.position = pose.LeftHandPosition;
            _leftHandTarget.rotation = pose.LeftHand;

            _rightShoulderTarget.position = pose.RightShoulderPosition;
            _rightElbowTarget.position = pose.RightElbowPosition;
            _rightHandTarget.position = pose.RightHandPosition;
            _rightHandTarget.rotation = pose.RightHand;
        }

        void ApplyLocal(Animator animator, AvatarPoseData pose, HumanBodyBones boneName)
            => animator.GetBoneTransform(boneName).localRotation = pose[boneName];
        
        void ApplyGlobal(Animator animator, AvatarPoseData pose, HumanBodyBones boneName)
            => animator.GetBoneTransform(boneName).rotation = pose[boneName];

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
    }
}
