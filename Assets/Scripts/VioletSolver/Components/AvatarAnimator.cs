using System;
using System.Collections.Generic;
using UnityEngine;
using VRM;

using mpHand = HolisticPose.HandLandmarks.Types.LandmarkIndex;

// This is avatar animating component which does
//  1. gets landmarks and filters landmarks (in _landmarkHandler)
//  2. solve landmarks to avatar pose
//  3. filters pose and apply pose to avatar (in _avatarPoseHandler)
namespace VioletSolver {
    public class AvatarAnimator : MonoBehaviour
    {
        [SerializeField] LandmarkHandler _landmarkHandler;
        [SerializeField] AvatarPoseSolver _poseSolver;
        [SerializeField] AvatarPoseHandler _avatarPoseHandler;
        [SerializeField] Animator _animator;
        [SerializeField] VRMBlendShapeProxy _proxy;
        [SerializeField] bool _isAnimating = false;
        AvatarBonePositions _restBonePositions;

        DataGroups.HandData _leftHand;
        DataGroups.HandData _rightHand;

        public LandmarkHandler Landmarks => _landmarkHandler;

        private void Start()
        {
            //_landmarkHandler = new LandmarkHandler();
            //_poseSolver = new AvatarPoseSolver();
            //_avatarPoseHandler = new AvatarPoseHandler();
            SetBonePositions(_animator);

            _leftHand = new();
            _rightHand = new();
        }

        void Update() 
        { 
            if( _isAnimating)
            {
                _landmarkHandler.Update();
                _landmarkHandler.UpdateBlendshapes();
                var (existPose, existFace, existLeftHand, existRightHand) = (UpdatePose(), UpdateBlendshapes(), UpdateLeftHand(), UpdateRightHand());

                if(existPose)
                    AnimateAvatar(_animator, _avatarPoseHandler.PoseData);
                if (existFace)
                    AnimateFace(_proxy, _avatarPoseHandler.BlendshapeWeights);
                if (existLeftHand)
                    AnimateLeftHand(_animator, _leftHand);
                if (existRightHand)
                    AnimatRightHand(_animator, _rightHand);
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>Whether updating landmarks is processed properly or not.</returns>
        bool UpdatePose()
        {
            var landmarks = _landmarkHandler.Landmarks;
            if (landmarks == null || 
                landmarks.Pose == null ||
                landmarks.Pose.Landmarks == null ||
                landmarks.Pose.Landmarks.Count <= 0) 
                return false;
            var pose = AvatarPoseSolver.Solve(landmarks, _restBonePositions);
            _avatarPoseHandler.Update(pose);
            return true;
        }

        bool UpdateLeftHand()
        {
            var landmarks = _landmarkHandler.Landmarks;
            if (landmarks == null ||
                landmarks.LeftHand == null ||
                landmarks.LeftHand.Landmarks == null ||
                landmarks.LeftHand.Landmarks.Count <= 0)
                return false;
            _leftHand = HandResolver.SolveLeftHand(landmarks.LeftHand);
            return true;
        }

        bool UpdateRightHand()
        {
            var landmarks = _landmarkHandler.Landmarks;
            if (landmarks == null ||
                landmarks.RightHand == null ||
                landmarks.RightHand.Landmarks == null ||
                landmarks.RightHand.Landmarks.Count <= 0)
                return false;
            _rightHand = HandResolver.SolveRightHand(landmarks.RightHand);
            return true;
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

        void AnimateAvatar(Animator avatarAnimator, AvatarPoseData pose)
        {
            var hbb = Enum.GetValues(typeof(HumanBodyBones));
            foreach(var bone in hbb)
            {
                var boneName = (HumanBodyBones)bone;
                var rot = pose[boneName];

                if (boneName == HumanBodyBones.LeftEye
                    || boneName == HumanBodyBones.RightEye)
                {
                    try
                    {
                        avatarAnimator.GetBoneTransform(boneName).localRotation = rot;
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        avatarAnimator.GetBoneTransform(boneName).rotation = rot;
                    }
                    catch { }
                }
            }
        }

        void AnimateLeftHand(Animator animator, DataGroups.HandData hand)
        {
            animator.GetBoneTransform(HumanBodyBones.LeftHand).rotation = hand[(int)mpHand.Wrist];
            animator.GetBoneTransform(HumanBodyBones.LeftThumbProximal).localRotation = hand[(int)mpHand.ThumbCmc];
            animator.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate).localRotation = hand[(int)mpHand.ThumbMcp];
            animator.GetBoneTransform(HumanBodyBones.LeftThumbDistal).localRotation = hand[(int)mpHand.ThumbIp];
            animator.GetBoneTransform(HumanBodyBones.LeftIndexProximal).localRotation = hand[(int)mpHand.IndexFingerMcp];
            animator.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate).localRotation = hand[(int)mpHand.IndexFingerPip];
            animator.GetBoneTransform(HumanBodyBones.LeftIndexDistal).localRotation = hand[(int)mpHand.IndexFingerDip];
            animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal).localRotation = hand[(int)mpHand.MiddleFingerMcp];
            animator.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate).localRotation = hand[(int)mpHand.MiddleFingerPip];
            animator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal).localRotation = hand[(int)mpHand.MiddleFingerDip];
            animator.GetBoneTransform(HumanBodyBones.LeftRingProximal).localRotation = hand[(int)mpHand.RingFingerMcp];
            animator.GetBoneTransform(HumanBodyBones.LeftRingIntermediate).localRotation = hand[(int)mpHand.RingFingerPip];
            animator.GetBoneTransform(HumanBodyBones.LeftRingDistal).localRotation = hand[(int)mpHand.RingFingerDip];
            animator.GetBoneTransform(HumanBodyBones.LeftLittleProximal).localRotation = hand[(int)mpHand.PinkyMcp];
            animator.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate).localRotation = hand[(int)mpHand.PinkyPip];
            animator.GetBoneTransform(HumanBodyBones.LeftLittleDistal).localRotation = hand[(int)mpHand.PinkyDip];
        }

        void AnimatRightHand(Animator animator, DataGroups.HandData hand)
        {
            animator.GetBoneTransform(HumanBodyBones.RightHand).rotation = hand[(int)mpHand.Wrist];
            animator.GetBoneTransform(HumanBodyBones.RightThumbProximal).localRotation = hand[(int)mpHand.ThumbCmc];
            animator.GetBoneTransform(HumanBodyBones.RightThumbIntermediate).localRotation = hand[(int)mpHand.ThumbMcp];
            animator.GetBoneTransform(HumanBodyBones.RightThumbDistal).localRotation = hand[(int)mpHand.ThumbIp];
            animator.GetBoneTransform(HumanBodyBones.RightIndexProximal).localRotation = hand[(int)mpHand.IndexFingerMcp];
            animator.GetBoneTransform(HumanBodyBones.RightIndexIntermediate).localRotation = hand[(int)mpHand.IndexFingerPip];
            animator.GetBoneTransform(HumanBodyBones.RightIndexDistal).localRotation = hand[(int)mpHand.IndexFingerDip];
            animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal).localRotation = hand[(int)mpHand.MiddleFingerMcp];
            animator.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate).localRotation = hand[(int)mpHand.MiddleFingerPip];
            animator.GetBoneTransform(HumanBodyBones.RightMiddleDistal).localRotation = hand[(int)mpHand.MiddleFingerDip];
            animator.GetBoneTransform(HumanBodyBones.RightRingProximal).localRotation = hand[(int)mpHand.RingFingerMcp];
            animator.GetBoneTransform(HumanBodyBones.RightRingIntermediate).localRotation = hand[(int)mpHand.RingFingerPip];
            animator.GetBoneTransform(HumanBodyBones.RightRingDistal).localRotation = hand[(int)mpHand.RingFingerDip];
            animator.GetBoneTransform(HumanBodyBones.RightLittleProximal).localRotation = hand[(int)mpHand.PinkyMcp];
            animator.GetBoneTransform(HumanBodyBones.RightLittleIntermediate).localRotation = hand[(int)mpHand.PinkyPip];
            animator.GetBoneTransform(HumanBodyBones.RightLittleDistal).localRotation = hand[(int)mpHand.PinkyDip];
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
