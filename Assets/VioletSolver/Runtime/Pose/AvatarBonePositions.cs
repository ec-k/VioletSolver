using UnityEngine;

namespace VioletSolver.Pose
{
    // TODO: This class must have operator.
    internal struct AvatarBonePositions
    {
        internal Vector3 this[HumanBodyBones index]
        {
            get
            {
                return index switch
                {
                    HumanBodyBones.Hips => Hips,
                    HumanBodyBones.Spine => Spine,
                    HumanBodyBones.Chest => Chest,
                    HumanBodyBones.UpperChest => UpperChest,
                    HumanBodyBones.Neck => Neck,
                    HumanBodyBones.Head => Head,

                    HumanBodyBones.LeftUpperLeg => LeftUpperLeg,
                    HumanBodyBones.RightUpperLeg => RightUpperLeg,
                    HumanBodyBones.LeftLowerLeg => LeftLowerLeg,
                    HumanBodyBones.RightLowerLeg => RightLowerLeg,
                    HumanBodyBones.LeftFoot => LeftFoot,
                    HumanBodyBones.RightFoot => RightFoot,

                    HumanBodyBones.LeftShoulder => LeftShoulder,
                    HumanBodyBones.RightShoulder => RightShoulder,
                    HumanBodyBones.LeftUpperArm => LeftUpperArm,
                    HumanBodyBones.RightUpperArm => RightUpperArm,
                    HumanBodyBones.LeftLowerArm => LeftLowerArm,
                    HumanBodyBones.RightLowerArm => RightLowerArm,
                    HumanBodyBones.LeftHand => LeftHand,
                    HumanBodyBones.RightHand => RightHand,

                    _ => Vector3.zero
                };
            }
        }

        internal Vector3 Hips;
        internal Vector3 Spine;
        internal Vector3 Chest;
        internal Vector3 UpperChest;
        internal Vector3 Neck;
        internal Vector3 Head;

        internal Vector3 LeftUpperLeg;
        internal Vector3 RightUpperLeg;
        internal Vector3 LeftLowerLeg;
        internal Vector3 RightLowerLeg;
        internal Vector3 LeftFoot;
        internal Vector3 RightFoot;

        internal Vector3 LeftShoulder;
        internal Vector3 RightShoulder;
        internal Vector3 LeftUpperArm;
        internal Vector3 RightUpperArm;
        internal Vector3 LeftLowerArm;
        internal Vector3 RightLowerArm;
        internal Vector3 LeftHand;
        internal Vector3 RightHand;

        public AvatarBonePositions(
            Vector3 hips, Vector3 spine, Vector3 chest, Vector3 upperChest, Vector3 neck, Vector3 head,
            Vector3 leftUpperLeg, Vector3 rightUpperLeg, Vector3 leftLowerLeg, Vector3 rightLowerLeg,
            Vector3 leftFoot, Vector3 rightFoot,
            Vector3 leftShoulder, Vector3 rightShoulder, Vector3 leftUpperArm, Vector3 rightUpperArm,
            Vector3 leftLowerArm, Vector3 rightLowerArm, Vector3 leftHand, Vector3 rightHand)
        {
            Hips = hips;
            Spine = spine;
            Chest = chest;
            UpperChest = upperChest;
            Neck = neck;
            Head = head;

            LeftUpperLeg = leftUpperLeg;
            RightUpperLeg = rightUpperLeg;
            LeftLowerLeg = leftLowerLeg;
            RightLowerLeg = rightLowerLeg;
            LeftFoot = leftFoot;
            RightFoot = rightFoot;

            LeftShoulder = leftShoulder;
            RightShoulder = rightShoulder;
            LeftUpperArm = leftUpperArm;
            RightUpperArm = rightUpperArm;
            LeftLowerArm = leftLowerArm;
            RightLowerArm = rightLowerArm;
            LeftHand = leftHand;
            RightHand = rightHand;
        }
    }
}
