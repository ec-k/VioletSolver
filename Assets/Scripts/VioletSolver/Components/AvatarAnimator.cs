using System;
using UnityEngine;
using VioletSolver.Network;

// This is avatar animating component which does
//  1. gets landmarks and filters landmarks (in _landmarkHandler)
//  2. solve landmarks to avatar pose
//  3. filters pose and apply pose to avatar (in _avatarPoseHandler)
namespace VioletSolver {
    public class AvatarAnimator : MonoBehaviour
    {
        [SerializeField] LandmarkHandler _landmarkHandler;
        [SerializeField] AvatarPoseHandler _avatarPoseHandler;
        [SerializeField] Animator _avatarAnimator;

        void Start() { }
        void Update() { }

        void AnimateAvatar(Animator avatarAnimator, AvatarPoseData pose)
        {
            var hbb = Enum.GetValues(typeof(HumanBodyBones));
            foreach(var bone in hbb)
            {
                var boneName = (HumanBodyBones)bone;
                var rot = pose.BodyBones(boneName);

                AnimateBone(avatarAnimator, boneName, rot);
            }
        }

        // To add interpolation or motion filtering later easily, I separated this process as a function.
        void AnimateBone(Animator avatarAnimator, HumanBodyBones boneName, Quaternion rotation)
        {
            avatarAnimator.GetBoneTransform(boneName).localRotation = rotation;
        }
    }
}
