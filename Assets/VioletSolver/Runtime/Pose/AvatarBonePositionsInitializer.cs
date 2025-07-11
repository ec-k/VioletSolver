using UnityEngine;

namespace VioletSolver.Pose
{
    internal static class AvatarBonePositionsInitializer
    {
        internal static AvatarBonePositions CreateFromAnimator(Animator animator)
        {
            Vector3 hips = GetBonePositionSafe(animator, HumanBodyBones.Hips);
            Vector3 spine = GetBonePositionSafe(animator, HumanBodyBones.Spine);
            Vector3 chest = GetBonePositionSafe(animator, HumanBodyBones.Chest);
            Vector3 upperChest = GetBonePositionSafe(animator, HumanBodyBones.UpperChest);
            Vector3 neck = GetBonePositionSafe(animator, HumanBodyBones.Neck);
            Vector3 head = GetBonePositionSafe(animator, HumanBodyBones.Head);

            Vector3 leftUpperLeg = GetBonePositionSafe(animator, HumanBodyBones.LeftUpperLeg);
            Vector3 rightUpperLeg = GetBonePositionSafe(animator, HumanBodyBones.RightUpperLeg);
            Vector3 leftLowerLeg = GetBonePositionSafe(animator, HumanBodyBones.LeftLowerLeg);
            Vector3 rightLowerLeg = GetBonePositionSafe(animator, HumanBodyBones.RightLowerLeg);
            Vector3 leftFoot = GetBonePositionSafe(animator, HumanBodyBones.LeftFoot);
            Vector3 rightFoot = GetBonePositionSafe(animator, HumanBodyBones.RightFoot);

            Vector3 leftShoulder = GetBonePositionSafe(animator, HumanBodyBones.LeftShoulder);
            Vector3 rightShoulder = GetBonePositionSafe(animator, HumanBodyBones.RightShoulder);
            Vector3 leftUpperArm = GetBonePositionSafe(animator, HumanBodyBones.LeftUpperArm);
            Vector3 rightUpperArm = GetBonePositionSafe(animator, HumanBodyBones.RightUpperArm);
            Vector3 leftLowerArm = GetBonePositionSafe(animator, HumanBodyBones.LeftLowerArm);
            Vector3 rightLowerArm = GetBonePositionSafe(animator, HumanBodyBones.RightLowerArm);
            Vector3 leftHand = GetBonePositionSafe(animator, HumanBodyBones.LeftHand);
            Vector3 rightHand = GetBonePositionSafe(animator, HumanBodyBones.RightHand);

            return new AvatarBonePositions(
                hips, spine, chest, upperChest, neck, head,
                leftUpperLeg, rightUpperLeg, leftLowerLeg, rightLowerLeg,
                leftFoot, rightFoot,
                leftShoulder, rightShoulder, leftUpperArm, rightUpperArm,
                leftLowerArm, rightLowerArm, leftHand, rightHand
            );
        }

        static Vector3 GetBonePositionSafe(Animator animator, HumanBodyBones bone)
        {
            var boneTransform = animator.GetBoneTransform(bone);
            return boneTransform != null ? boneTransform.position : Vector3.zero;
        }
    }
}