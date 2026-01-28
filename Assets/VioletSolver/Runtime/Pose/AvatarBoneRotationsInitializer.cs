using UnityEngine;

namespace VioletSolver.Pose
{
    internal static class AvatarBoneRotationsInitializer
    {
        internal static AvatarBoneRotations CreateFromAnimator(Animator animator)
        {
            var rootPos = animator.rootPosition;
            var rootRotInv = Quaternion.Inverse(animator.rootRotation);

            var hips = GetNormalizedRotation(animator, HumanBodyBones.Hips, rootRotInv);
            var spine = GetNormalizedRotation(animator, HumanBodyBones.Spine, rootRotInv);
            var chest = GetNormalizedRotation(animator, HumanBodyBones.Chest, rootRotInv);
            var upperChest = GetNormalizedRotation(animator, HumanBodyBones.UpperChest, rootRotInv);
            var neck = GetNormalizedRotation(animator, HumanBodyBones.Neck, rootRotInv);
            var head = GetNormalizedRotation(animator, HumanBodyBones.Head, rootRotInv);

            var leftUpperLeg = GetNormalizedRotation(animator, HumanBodyBones.LeftUpperLeg, rootRotInv);
            var rightUpperLeg = GetNormalizedRotation(animator, HumanBodyBones.RightUpperLeg, rootRotInv);
            var leftLowerLeg = GetNormalizedRotation(animator, HumanBodyBones.LeftLowerLeg, rootRotInv);
            var rightLowerLeg = GetNormalizedRotation(animator, HumanBodyBones.RightLowerLeg, rootRotInv);
            var leftFoot = GetNormalizedRotation(animator, HumanBodyBones.LeftFoot, rootRotInv);
            var rightFoot = GetNormalizedRotation(animator, HumanBodyBones.RightFoot, rootRotInv);

            var leftShoulder = GetNormalizedRotation(animator, HumanBodyBones.LeftShoulder, rootRotInv);
            var rightShoulder = GetNormalizedRotation(animator, HumanBodyBones.RightShoulder, rootRotInv);
            var leftUpperArm = GetNormalizedRotation(animator, HumanBodyBones.LeftUpperArm, rootRotInv);
            var rightUpperArm = GetNormalizedRotation(animator, HumanBodyBones.RightUpperArm, rootRotInv);
            var leftLowerArm = GetNormalizedRotation(animator, HumanBodyBones.LeftLowerArm, rootRotInv);
            var rightLowerArm = GetNormalizedRotation(animator, HumanBodyBones.RightLowerArm, rootRotInv);
            var leftHand = GetNormalizedRotation(animator, HumanBodyBones.LeftHand, rootRotInv);
            var rightHand = GetNormalizedRotation(animator, HumanBodyBones.RightHand, rootRotInv);

            return new AvatarBoneRotations(
                hips, spine, chest, upperChest, neck, head,
                leftUpperLeg, rightUpperLeg, leftLowerLeg, rightLowerLeg,
                leftFoot, rightFoot,
                leftShoulder, rightShoulder, leftUpperArm, rightUpperArm,
                leftLowerArm, rightLowerArm, leftHand, rightHand
            );
        }

        static Quaternion GetNormalizedRotation(Animator animator, HumanBodyBones boneName, Quaternion rootRotInverse)
        {
            var boneTransform = animator.GetBoneTransform(boneName);
            if (boneTransform == null) return Quaternion.identity;

            return rootRotInverse * boneTransform.rotation;
        }
    }
}