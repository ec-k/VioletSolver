using UnityEngine;

namespace VioletSolver.Pose
{
    internal static class AvatarBonesInitializer
    {
        internal static AvatarBones CreateFromAnimator(Animator animator)
        {
            var rootPos = animator.rootPosition;
            var rootRotInv = Quaternion.Inverse(animator.rootRotation);

            var hips = GetNormalizedBonePose(animator, HumanBodyBones.Hips, rootPos, rootRotInv);
            var spine = GetNormalizedBonePose(animator, HumanBodyBones.Spine, rootPos, rootRotInv);
            var chest = GetNormalizedBonePose(animator, HumanBodyBones.Chest, rootPos, rootRotInv);
            var upperChest = GetNormalizedBonePose(animator, HumanBodyBones.UpperChest, rootPos, rootRotInv);
            var neck = GetNormalizedBonePose(animator, HumanBodyBones.Neck, rootPos, rootRotInv);
            var head = GetNormalizedBonePose(animator, HumanBodyBones.Head, rootPos, rootRotInv);

            var leftUpperLeg = GetNormalizedBonePose(animator, HumanBodyBones.LeftUpperLeg, rootPos, rootRotInv);
            var rightUpperLeg = GetNormalizedBonePose(animator, HumanBodyBones.RightUpperLeg, rootPos, rootRotInv);
            var leftLowerLeg = GetNormalizedBonePose(animator, HumanBodyBones.LeftLowerLeg, rootPos, rootRotInv);
            var rightLowerLeg = GetNormalizedBonePose(animator, HumanBodyBones.RightLowerLeg, rootPos, rootRotInv);
            var leftFoot = GetNormalizedBonePose(animator, HumanBodyBones.LeftFoot, rootPos, rootRotInv);
            var rightFoot = GetNormalizedBonePose(animator, HumanBodyBones.RightFoot, rootPos, rootRotInv);

            var leftShoulder = GetNormalizedBonePose(animator, HumanBodyBones.LeftShoulder, rootPos, rootRotInv);
            var rightShoulder = GetNormalizedBonePose(animator, HumanBodyBones.RightShoulder, rootPos, rootRotInv);
            var leftUpperArm = GetNormalizedBonePose(animator, HumanBodyBones.LeftUpperArm, rootPos, rootRotInv);
            var rightUpperArm = GetNormalizedBonePose(animator, HumanBodyBones.RightUpperArm, rootPos, rootRotInv);
            var leftLowerArm = GetNormalizedBonePose(animator, HumanBodyBones.LeftLowerArm, rootPos, rootRotInv);
            var rightLowerArm = GetNormalizedBonePose(animator, HumanBodyBones.RightLowerArm, rootPos, rootRotInv);
            var leftHand = GetNormalizedBonePose(animator, HumanBodyBones.LeftHand, rootPos, rootRotInv);
            var rightHand = GetNormalizedBonePose(animator, HumanBodyBones.RightHand, rootPos, rootRotInv);

            return new AvatarBones(
                hips, spine, chest, upperChest, neck, head,
                leftUpperLeg, rightUpperLeg, leftLowerLeg, rightLowerLeg,
                leftFoot, rightFoot,
                leftShoulder, rightShoulder, leftUpperArm, rightUpperArm,
                leftLowerArm, rightLowerArm, leftHand, rightHand
            );
        }

        static BonePose GetNormalizedBonePose(Animator animator, HumanBodyBones boneName, Vector3 rootPos, Quaternion rootRotInverse)
        {
            var boneTransform = animator.GetBoneTransform(boneName);
            if (boneTransform == null)
                return new BonePose { Position = Vector3.zero, Rotation = Quaternion.identity };

            return new BonePose
            {
                Position = rootRotInverse * (boneTransform.position - rootPos),
                Rotation = rootRotInverse * boneTransform.rotation
            };
        }
    }
}
