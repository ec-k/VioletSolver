using System;
using System.Collections.Generic;
using UnityEngine;
using VRM;

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
                var isUpdated = UpdatePose();
                if (!isUpdated)
                    return;

                isUpdated = UpdateBlendshapes();
                if(!isUpdated) 
                    return;

                var pose = _avatarPoseHandler.PoseData;
                var blendshapes = _avatarPoseHandler.BlendshapeWeights;

                AnimateAvatar(_animator, pose);
                AnimateFace(_proxy, blendshapes);
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>Whether updating landmarks is processed properly or not.</returns>
        bool UpdatePose()
        {
            _landmarkHandler.Update();
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

        bool UpdateBlendshapes()
        {
            _landmarkHandler.UpdateBlendshapes();
            var mpBlendshapes = _landmarkHandler.MpBlendshapes;
            if (mpBlendshapes == null ||
                mpBlendshapes == null ||
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
