using System;
using System.Collections.Generic;
using UnityEngine;
using VRM;

using VioletSolver.Pose;
using VioletSolver.Solver;
using mpBlendshapes = HolisticPose.Blendshapes.Types.BlendshapesIndex;
using RootMotion.FinalIK;

// This is avatar animating component which does
//  1. gets landmarks and filters landmarks (in _landmarkHandler)
//  2. solve landmarks to avatar pose
//  3. filters pose and apply pose to avatar (in _avatarPoseHandler)
namespace VioletSolver 
{
    public class AvatarAnimator : MonoBehaviour
    {
        [SerializeField] LandmarkHandler _landmarkHandler;
        [SerializeField] PoseHandler _avatarPoseHandler;
        [SerializeField] Animator _animator;
        [SerializeField] VRMBlendShapeProxy _proxy;
        [SerializeField] bool _isAnimating = false;
        [SerializeField] bool _animateLeg = false;
        AvatarBonePositions _restBonePositions;

        [SerializeField] bool _enablePerfectSync = false;


        [SerializeField]bool _useIk = true;

        ArmIK _leftArmIk;
        ArmIK _rightArmIk;
        // IK Targets
        Transform _leftShoulderTarget;
        Transform _leftElbowTarget;
        Transform _leftHandTarget;
        Transform _rightShoulderTarget;
        Transform _rightElbowTarget;
        Transform _rightHandTarget;

        public LandmarkHandler Landmarks => _landmarkHandler;

        private void Start()
        {
            SetBonePositions(_animator);

            SetupIkTargets();
            SetupArmIk(true);
            SetupArmIk(false);
        }

        void SetupIkTargets()
        {
            var targetRoot = new GameObject("ArmIK Targets").transform;
            targetRoot.parent = gameObject.transform;

            SetupIkTarget(targetRoot, "LeftShoulderTarget", ref _leftShoulderTarget);
            SetupIkTarget(targetRoot, "LeftElbowTarget", ref _leftElbowTarget);
            SetupIkTarget(targetRoot, "LeftHandTarget", ref _leftHandTarget);
            SetupIkTarget(targetRoot, "RightShoulderTarget", ref _rightShoulderTarget);
            SetupIkTarget(targetRoot, "RightElbowTarget", ref _rightElbowTarget);
            SetupIkTarget(targetRoot, "RightHandTarget", ref _rightHandTarget);
        }

        void SetupIkTarget(Transform root, string name, ref Transform target)
        {
            target = new GameObject(name).transform;
            target.parent = root;
        }

        void SetupArmIk(bool isLeft) 
        {            
            var armIk = gameObject.AddComponent<ArmIK>(); ;

            armIk.solver.IKPositionWeight = 1f;
            armIk.solver.IKRotationWeight = 1f;
            armIk.solver.arm.bendGoalWeight = 1f;
            armIk.solver.chest.transform = _animator.GetBoneTransform(HumanBodyBones.Chest);

            if (isLeft) 
            {
                _leftArmIk = armIk;

                armIk.solver.shoulder.transform = _animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
                armIk.solver.upperArm.transform = _animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
                armIk.solver.forearm.transform  = _animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                armIk.solver.hand.transform     = _animator.GetBoneTransform(HumanBodyBones.LeftHand);

                armIk.solver.arm.target     = _leftHandTarget;
                armIk.solver.arm.bendGoal   = _leftElbowTarget;
                armIk.solver.isLeft         = true;
            }
            else
            {
                _rightArmIk = armIk;

                armIk.solver.shoulder.transform = _animator.GetBoneTransform(HumanBodyBones.RightShoulder);
                armIk.solver.upperArm.transform = _animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
                armIk.solver.forearm.transform = _animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
                armIk.solver.hand.transform = _animator.GetBoneTransform(HumanBodyBones.RightHand);

                armIk.solver.arm.target     = _rightHandTarget;
                armIk.solver.arm.bendGoal   = _rightElbowTarget;
                armIk.solver.isLeft         = false;
            }

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
                if (_enablePerfectSync)
                    UpdateBlendshapesPerfectly();
                else
                    UpdateBlendshapes();

                AnimateAvatar(_animator, _avatarPoseHandler.PoseData);
                if (_enablePerfectSync)
                    AnimateFace(_proxy, _avatarPoseHandler.PerfectSyncWeights);
                else
                    AnimateFace(_proxy, _avatarPoseHandler.BlendshapeWeights);
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
            
            ApplyLocal(animator, pose, HumanBodyBones.LeftEye);
            ApplyLocal(animator, pose, HumanBodyBones.RightEye);
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
