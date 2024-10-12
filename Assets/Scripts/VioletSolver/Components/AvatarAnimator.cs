using System;
using UnityEngine;

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
                var pose = _avatarPoseHandler.PoseData;
                AnimateAvatar(_animator, pose);
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
                landmarks.Landmarks == null ||
                landmarks.Landmarks.Count <= 0) 
                return false;
            var pose = AvatarPoseSolver.Solve(landmarks, _restBonePositions);
            _avatarPoseHandler.Update(pose);
            return true;
        }

        void AnimateAvatar(Animator avatarAnimator, AvatarPoseData pose)
        {
            var hbb = Enum.GetValues(typeof(HumanBodyBones));
            foreach(var bone in hbb)
            {
                var boneName = (HumanBodyBones)bone;
                var rot = pose.BodyBones(boneName);

                try
                {
                    AnimateBone(avatarAnimator, boneName, rot);
                }
                catch { }
            }
        }

        // To add interpolation or motion filtering later easily, I separated this process as a function.
        void AnimateBone(Animator avatarAnimator, HumanBodyBones boneName, Quaternion rotation)
        {
            avatarAnimator.GetBoneTransform(boneName).rotation = rotation;
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
