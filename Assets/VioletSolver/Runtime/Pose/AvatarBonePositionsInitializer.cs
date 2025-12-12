using UnityEngine;

namespace VioletSolver.Pose
{
    internal static class AvatarBonePositionsInitializer
    {
        internal static AvatarBonePositions CreateFromAnimator(Animator animator)
        {
            var rootPos = animator.rootPosition;
            var rootRotInv = Quaternion.Inverse(animator.rootRotation);

            var hips = GetNormalizedPosition(animator, HumanBodyBones.Hips, rootPos, rootRotInv);
            var spine = GetNormalizedPosition(animator, HumanBodyBones.Spine, rootPos, rootRotInv);
            var chest = GetNormalizedPosition(animator, HumanBodyBones.Chest, rootPos, rootRotInv);
            var upperChest = GetNormalizedPosition(animator, HumanBodyBones.UpperChest, rootPos, rootRotInv);
            var neck = GetNormalizedPosition(animator, HumanBodyBones.Neck, rootPos, rootRotInv);
            var head = GetNormalizedPosition(animator, HumanBodyBones.Head, rootPos, rootRotInv);

            var leftUpperLeg = GetNormalizedPosition(animator, HumanBodyBones.LeftUpperLeg, rootPos, rootRotInv);
            var rightUpperLeg = GetNormalizedPosition(animator, HumanBodyBones.RightUpperLeg, rootPos, rootRotInv);
            var leftLowerLeg = GetNormalizedPosition(animator, HumanBodyBones.LeftLowerLeg, rootPos, rootRotInv);
            var rightLowerLeg = GetNormalizedPosition(animator, HumanBodyBones.RightLowerLeg, rootPos, rootRotInv);
            var leftFoot = GetNormalizedPosition(animator, HumanBodyBones.LeftFoot, rootPos, rootRotInv);
            var rightFoot = GetNormalizedPosition(animator, HumanBodyBones.RightFoot, rootPos, rootRotInv);

            var leftShoulder = GetNormalizedPosition(animator, HumanBodyBones.LeftShoulder, rootPos, rootRotInv);
            var rightShoulder = GetNormalizedPosition(animator, HumanBodyBones.RightShoulder, rootPos, rootRotInv);
            var leftUpperArm = GetNormalizedPosition(animator, HumanBodyBones.LeftUpperArm, rootPos, rootRotInv);
            var rightUpperArm = GetNormalizedPosition(animator, HumanBodyBones.RightUpperArm, rootPos, rootRotInv);
            var leftLowerArm = GetNormalizedPosition(animator, HumanBodyBones.LeftLowerArm, rootPos, rootRotInv);
            var rightLowerArm = GetNormalizedPosition(animator, HumanBodyBones.RightLowerArm, rootPos, rootRotInv);
            var leftHand = GetNormalizedPosition(animator, HumanBodyBones.LeftHand, rootPos, rootRotInv);
            var rightHand = GetNormalizedPosition(animator, HumanBodyBones.RightHand, rootPos, rootRotInv);

            return new AvatarBonePositions(
                hips, spine, chest, upperChest, neck, head,
                leftUpperLeg, rightUpperLeg, leftLowerLeg, rightLowerLeg,
                leftFoot, rightFoot,
                leftShoulder, rightShoulder, leftUpperArm, rightUpperArm,
                leftLowerArm, rightLowerArm, leftHand, rightHand
            );
        }

        static Vector3 GetNormalizedPosition(Animator animator, HumanBodyBones boneName, Vector3 rootPos, Quaternion rootRotInverse)
        {
            var boneTransform = animator.GetBoneTransform(boneName);
            if (boneTransform == null) return Vector3.zero;

            return  rootRotInverse * (boneTransform.position - rootPos);
        }
    }
}