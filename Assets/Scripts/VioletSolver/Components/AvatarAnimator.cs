using HolisticPose;
using System;
using System.Collections.Generic;
using UnityEngine;
using VRM;

using mpBlendshapes = HolisticPose.Blendshapes.Types.BlendshapesIndex;
using mpHand = HolisticPose.HandLandmarks.Types.LandmarkIndex;

// This is avatar animating component which does
//  1. gets landmarks and filters landmarks (in _landmarkHandler)
//  2. solve landmarks to avatar pose
//  3. filters pose and apply pose to avatar (in _avatarPoseHandler)
namespace VioletSolver {
    public class AvatarAnimator : MonoBehaviour
    {
        [SerializeField] LandmarkHandler _landmarkHandler;
        [SerializeField] AvatarPoseHandler _avatarPoseHandler;
        [SerializeField] Animator _animator;
        [SerializeField] VRMBlendShapeProxy _proxy;
        [SerializeField] bool _isAnimating = false;
        AvatarBonePositions _restBonePositions;

        [SerializeField] bool _enablePerfectSync = false;

        public LandmarkHandler Landmarks => _landmarkHandler;

        private void Start()
        {
            //_landmarkHandler = new LandmarkHandler();
            //_poseSolver = new AvatarPoseSolver();
            //_avatarPoseHandler = new AvatarPoseHandler();
            SetBonePositions(_animator);
        }

        void Update() 
        { 
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
            var pose = AvatarPoseSolver.Solve(landmarks, _restBonePositions);
            _avatarPoseHandler.Update(pose);
        }

        bool UpdateBlendshapes()
        {
            var mpBlendshapes = _landmarkHandler.MpBlendshapes;
            if (mpBlendshapes == null ||
                mpBlendshapes.Count <= 0)
                return false;
            var (blendshapes, leftEye, rightEye) = AvatarPoseSolver.Solve(mpBlendshapes);
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
            var (blendshapes, leftEye, rightEye) = AvatarPoseSolver.SolvePerfectly(mpBlendshapes);
            _avatarPoseHandler.Update(blendshapes);

            _avatarPoseHandler.Update(HumanBodyBones.LeftEye, leftEye);
            _avatarPoseHandler.Update(HumanBodyBones.RightEye, rightEye);

            return true;
        }

        void AnimateAvatar(Animator animator, AvatarPoseData pose)
        {
            ApplyGlobal(animator, pose, HumanBodyBones.Hips);
            ApplyGlobal(animator, pose, HumanBodyBones.Spine );
            ApplyGlobal(animator, pose, HumanBodyBones.Chest );
            ApplyGlobal(animator, pose, HumanBodyBones.UpperChest );
            ApplyGlobal(animator, pose, HumanBodyBones.Neck );
            ApplyGlobal(animator, pose, HumanBodyBones.Head );
            ApplyGlobal(animator, pose, HumanBodyBones.LeftUpperLeg );
            ApplyGlobal(animator, pose, HumanBodyBones.RightUpperLeg );
            ApplyGlobal(animator, pose, HumanBodyBones.LeftLowerLeg );
            ApplyGlobal(animator, pose, HumanBodyBones.RightLowerLeg );
            ApplyGlobal(animator, pose, HumanBodyBones.LeftFoot );
            ApplyGlobal(animator, pose, HumanBodyBones.RightFoot );
            ApplyGlobal(animator, pose, HumanBodyBones.LeftShoulder );
            ApplyGlobal(animator, pose, HumanBodyBones.RightShoulder );
            ApplyGlobal(animator, pose, HumanBodyBones.LeftUpperArm );
            ApplyGlobal(animator, pose, HumanBodyBones.RightUpperArm );
            ApplyGlobal(animator, pose, HumanBodyBones.LeftLowerArm );
            ApplyGlobal(animator, pose, HumanBodyBones.RightLowerArm );
            ApplyGlobal(animator, pose, HumanBodyBones.LeftHand);
            ApplyGlobal(animator, pose, HumanBodyBones.RightHand);

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
