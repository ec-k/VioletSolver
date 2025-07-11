using System;
using System.Collections.Generic;
using UnityEngine;
using VRM;

using VioletSolver.Pose;
using VioletSolver.Solver;
using mpBlendshapes = HolisticPose.Blendshapes.Types.BlendshapesIndex;
using RootMotion.FinalIK;

using VioletSolver.Network;

// This is avatar animating component which does
//  1. gets landmarks and filters landmarks (in _landmarkHandler)
//  2. solve landmarks to avatar pose
//  3. filters pose and apply pose to avatar (in _avatarPoseHandler)
namespace VioletSolver 
{
    [Serializable]
    public class AvatarAnimator
    {
        [SerializeField] GameObject _ikRigRoot;
        [SerializeField] Animator _animator;
        [SerializeField] VRMBlendShapeProxy _blendshapeProxy;
        [SerializeField] bool _isAnimating = false;
        [SerializeField] bool _animateLeg = false;
        [SerializeField] bool _isPerfectSyncEnabled = false;
        [SerializeField] bool _useIk = true;
        
        public LandmarkHandler Landmarks => _landmarkHandler;
        LandmarkHandler _landmarkHandler;
        PoseHandler _avatarPoseHandler;
        AvatarBonePositions _restBonePositions;

        ArmIK _leftArmIk;
        ArmIK _rightArmIk;
        // IK Targets
        Transform _leftShoulderTarget;
        Transform _leftElbowTarget;
        Transform _leftHandTarget;
        Transform _rightShoulderTarget;
        Transform _rightElbowTarget;
        Transform _rightHandTarget;

        float _timer = 0f;
        float _thresholdTime = 1f;
        bool _doOverride = false;

        public AvatarAnimator()
        {
            _landmarkHandler = new();
            _avatarPoseHandler = new();

            SetBonePositions(_animator);

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

        void Update() 
        {
            {
                _leftArmIk.enabled = _useIk;
                _rightArmIk.enabled = _useIk;
            }
            if( _isAnimating)
            {
                _landmarkHandler.Update(); 
                _landmarkHandler.UpdateBlendshapes();

                UpdatePose();
                if (_isPerfectSyncEnabled)
                    UpdateBlendshapesPerfectly();
                else
                    UpdateBlendshapes();

                AnimateAvatar(_animator, _avatarPoseHandler.PoseData);
                if (_isPerfectSyncEnabled)
                    AnimateFace(_blendshapeProxy, _avatarPoseHandler.PerfectSyncWeights);
                else
                    AnimateFace(_blendshapeProxy, _avatarPoseHandler.BlendshapeWeights);
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>Whether updating landmarks is processed properly or not.</returns>
        void UpdatePose()
        {
            var landmarks = _landmarkHandler.Landmarks;
            var pose = HolisticSolver.Solve(landmarks, _restBonePositions, _useIk);
            _avatarPoseHandler.Update(pose);
        }
        bool UpdateBlendshapes()
        {
            var mpBlendshapes = _landmarkHandler.MpBlendshapes;
            if (mpBlendshapes == null ||
                mpBlendshapes.Count <= 0)
                return false;
            var (blendshapes, leftEye, rightEye) = HolisticSolver.Solve(mpBlendshapes);
            _avatarPoseHandler.Update(blendshapes);

            _avatarPoseHandler.Update(HumanBodyBones.LeftEye, leftEye);
            _avatarPoseHandler.Update(HumanBodyBones.RightEye, rightEye);

            return true;
        }
        bool UpdateBlendshapesPerfectly()
        {
            var mpBlendshapes = _landmarkHandler.MpBlendshapes;
            if (mpBlendshapes == null ||
                mpBlendshapes.Count <= 0)
                return false;
            var (blendshapes, leftEye, rightEye) = HolisticSolver.SolvePerfectly(mpBlendshapes);
            _avatarPoseHandler.Update(blendshapes);

            _avatarPoseHandler.Update(HumanBodyBones.LeftEye, leftEye);
            _avatarPoseHandler.Update(HumanBodyBones.RightEye, rightEye);

            return true;
        }

        void AnimateAvatar(Animator animator, AvatarPoseData pose)
        {
            animator.GetBoneTransform(HumanBodyBones.Hips).position = pose.HipsPosition;

            ApplyGlobal(animator, pose, HumanBodyBones.Hips);
            ApplyGlobal(animator, pose, HumanBodyBones.Spine );
            ApplyGlobal(animator, pose, HumanBodyBones.Chest );
            ApplyGlobal(animator, pose, HumanBodyBones.UpperChest );
            ApplyGlobal(animator, pose, HumanBodyBones.Neck );
            ApplyGlobal(animator, pose, HumanBodyBones.Head );
            if (_animateLeg)
            {
                ApplyGlobal(animator, pose, HumanBodyBones.LeftUpperLeg);
                ApplyGlobal(animator, pose, HumanBodyBones.RightUpperLeg);
                ApplyGlobal(animator, pose, HumanBodyBones.LeftLowerLeg);
                ApplyGlobal(animator, pose, HumanBodyBones.RightLowerLeg);
                ApplyGlobal(animator, pose, HumanBodyBones.LeftFoot);
                ApplyGlobal(animator, pose, HumanBodyBones.RightFoot);
            }
            if (_useIk)
                ApplyIkTarget(pose);
            else
            {
                ApplyGlobal(animator, pose, HumanBodyBones.LeftShoulder);
                ApplyGlobal(animator, pose, HumanBodyBones.RightShoulder);
                ApplyGlobal(animator, pose, HumanBodyBones.LeftUpperArm);
                ApplyGlobal(animator, pose, HumanBodyBones.RightUpperArm);
                ApplyGlobal(animator, pose, HumanBodyBones.LeftLowerArm);
                ApplyGlobal(animator, pose, HumanBodyBones.RightLowerArm);
                ApplyGlobal(animator, pose, HumanBodyBones.LeftHand);
                ApplyGlobal(animator, pose, HumanBodyBones.RightHand);
            }
            
            ApplyLocal(animator, pose, HumanBodyBones.LeftEye);
            ApplyLocal(animator, pose, HumanBodyBones.RightEye);

            if (_doOverride)
            {
                _timer += Time.deltaTime;
                if (_timer >= _thresholdTime)
                {
                    _doOverride = false;

                }

                try
                {
                    // from external output (OSC)
                    animator.GetBoneTransform(HumanBodyBones.LeftThumbProximal).localRotation = _poseReceiver.Results[HumanBodyBones.LeftThumbProximal];
                    animator.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate).localRotation = _poseReceiver.Results[HumanBodyBones.LeftThumbIntermediate];
                    animator.GetBoneTransform(HumanBodyBones.LeftThumbDistal).localRotation = _poseReceiver.Results[HumanBodyBones.LeftThumbDistal];
                    animator.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate).localRotation = _poseReceiver.Results[HumanBodyBones.LeftIndexIntermediate];
                    animator.GetBoneTransform(HumanBodyBones.LeftIndexProximal).localRotation = _poseReceiver.Results[HumanBodyBones.LeftIndexProximal];
                    animator.GetBoneTransform(HumanBodyBones.LeftIndexDistal).localRotation = _poseReceiver.Results[HumanBodyBones.LeftIndexDistal];
                    animator.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate).localRotation = _poseReceiver.Results[HumanBodyBones.LeftMiddleIntermediate];
                    animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal).localRotation = _poseReceiver.Results[HumanBodyBones.LeftMiddleProximal];
                    animator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal).localRotation = _poseReceiver.Results[HumanBodyBones.LeftMiddleDistal];
                    animator.GetBoneTransform(HumanBodyBones.LeftRingIntermediate).localRotation = _poseReceiver.Results[HumanBodyBones.LeftRingIntermediate];
                    animator.GetBoneTransform(HumanBodyBones.LeftRingProximal).localRotation = _poseReceiver.Results[HumanBodyBones.LeftRingProximal];
                    animator.GetBoneTransform(HumanBodyBones.LeftRingDistal).localRotation = _poseReceiver.Results[HumanBodyBones.LeftRingDistal];
                    animator.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate).localRotation = _poseReceiver.Results[HumanBodyBones.LeftLittleIntermediate];
                    animator.GetBoneTransform(HumanBodyBones.LeftLittleProximal).localRotation = _poseReceiver.Results[HumanBodyBones.LeftLittleProximal];
                    animator.GetBoneTransform(HumanBodyBones.LeftLittleDistal).localRotation = _poseReceiver.Results[HumanBodyBones.LeftLittleDistal];

                    animator.GetBoneTransform(HumanBodyBones.RightThumbProximal).localRotation = _poseReceiver.Results[HumanBodyBones.RightThumbProximal];
                    animator.GetBoneTransform(HumanBodyBones.RightThumbIntermediate).localRotation = _poseReceiver.Results[HumanBodyBones.RightThumbIntermediate];
                    animator.GetBoneTransform(HumanBodyBones.RightThumbDistal).localRotation = _poseReceiver.Results[HumanBodyBones.RightThumbDistal];
                    animator.GetBoneTransform(HumanBodyBones.RightIndexIntermediate).localRotation = _poseReceiver.Results[HumanBodyBones.RightIndexIntermediate];
                    animator.GetBoneTransform(HumanBodyBones.RightIndexProximal).localRotation = _poseReceiver.Results[HumanBodyBones.RightIndexProximal];
                    animator.GetBoneTransform(HumanBodyBones.RightIndexDistal).localRotation = _poseReceiver.Results[HumanBodyBones.RightIndexDistal];
                    animator.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate).localRotation = _poseReceiver.Results[HumanBodyBones.RightMiddleIntermediate];
                    animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal).localRotation = _poseReceiver.Results[HumanBodyBones.RightMiddleProximal];
                    animator.GetBoneTransform(HumanBodyBones.RightMiddleDistal).localRotation = _poseReceiver.Results[HumanBodyBones.RightMiddleDistal];
                    animator.GetBoneTransform(HumanBodyBones.RightRingIntermediate).localRotation = _poseReceiver.Results[HumanBodyBones.RightRingIntermediate];
                    animator.GetBoneTransform(HumanBodyBones.RightRingProximal).localRotation = _poseReceiver.Results[HumanBodyBones.RightRingProximal];
                    animator.GetBoneTransform(HumanBodyBones.RightRingDistal).localRotation = _poseReceiver.Results[HumanBodyBones.RightRingDistal];
                    animator.GetBoneTransform(HumanBodyBones.RightLittleIntermediate).localRotation = _poseReceiver.Results[HumanBodyBones.RightLittleIntermediate];
                    animator.GetBoneTransform(HumanBodyBones.RightLittleProximal).localRotation = _poseReceiver.Results[HumanBodyBones.RightLittleProximal];
                    animator.GetBoneTransform(HumanBodyBones.RightLittleDistal).localRotation = _poseReceiver.Results[HumanBodyBones.RightLittleDistal];
                }
                catch {}
            }
            else
            {
                // from MediaPipe.Hand landmarks
                ApplyLocal(animator, pose, HumanBodyBones.LeftThumbProximal);
                ApplyLocal(animator, pose, HumanBodyBones.LeftThumbIntermediate);
                ApplyLocal(animator, pose, HumanBodyBones.LeftThumbDistal);
                ApplyLocal(animator, pose, HumanBodyBones.LeftIndexProximal);
                ApplyLocal(animator, pose, HumanBodyBones.LeftIndexIntermediate);
                ApplyLocal(animator, pose, HumanBodyBones.LeftIndexDistal);
                ApplyLocal(animator, pose, HumanBodyBones.LeftMiddleProximal);
                ApplyLocal(animator, pose, HumanBodyBones.LeftMiddleIntermediate);
                ApplyLocal(animator, pose, HumanBodyBones.LeftMiddleDistal);
                ApplyLocal(animator, pose, HumanBodyBones.LeftRingProximal);
                ApplyLocal(animator, pose, HumanBodyBones.LeftRingIntermediate);
                ApplyLocal(animator, pose, HumanBodyBones.LeftRingDistal);
                ApplyLocal(animator, pose, HumanBodyBones.LeftLittleProximal);
                ApplyLocal(animator, pose, HumanBodyBones.LeftLittleIntermediate);
                ApplyLocal(animator, pose, HumanBodyBones.LeftLittleDistal);

                ApplyLocal(animator, pose, HumanBodyBones.RightThumbProximal);
                ApplyLocal(animator, pose, HumanBodyBones.RightThumbIntermediate);
                ApplyLocal(animator, pose, HumanBodyBones.RightThumbDistal);
                ApplyLocal(animator, pose, HumanBodyBones.RightIndexProximal);
                ApplyLocal(animator, pose, HumanBodyBones.RightIndexIntermediate);
                ApplyLocal(animator, pose, HumanBodyBones.RightIndexDistal);
                ApplyLocal(animator, pose, HumanBodyBones.RightMiddleProximal);
                ApplyLocal(animator, pose, HumanBodyBones.RightMiddleIntermediate);
                ApplyLocal(animator, pose, HumanBodyBones.RightMiddleDistal);
                ApplyLocal(animator, pose, HumanBodyBones.RightRingProximal);
                ApplyLocal(animator, pose, HumanBodyBones.RightRingIntermediate);
                ApplyLocal(animator, pose, HumanBodyBones.RightRingDistal);
                ApplyLocal(animator, pose, HumanBodyBones.RightLittleProximal);
                ApplyLocal(animator, pose, HumanBodyBones.RightLittleIntermediate);
                ApplyLocal(animator, pose, HumanBodyBones.RightLittleDistal);
            }
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

        void SetBonePositions(Animator animator)
        {
            var hbb = Enum.GetValues(typeof(HumanBodyBones));
            foreach (var bone in hbb)
            {
                var boneName = (HumanBodyBones)bone;

                try
                {
                    var bonePos = animator.GetBoneTransform(boneName).position;
                    _restBonePositions[boneName] = bonePos;
                }
                catch { }
            }
        }
    }
}
