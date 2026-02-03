using UnityEngine;

namespace VioletSolver.Pose
{
    internal struct AvatarBones
    {
        internal BonePose this[HumanBodyBones index]
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

                    _ => default
                };
            }
        }

        internal BonePose Hips;
        internal BonePose Spine;
        internal BonePose Chest;
        internal BonePose UpperChest;
        internal BonePose Neck;
        internal BonePose Head;

        internal BonePose LeftUpperLeg;
        internal BonePose RightUpperLeg;
        internal BonePose LeftLowerLeg;
        internal BonePose RightLowerLeg;
        internal BonePose LeftFoot;
        internal BonePose RightFoot;

        internal BonePose LeftShoulder;
        internal BonePose RightShoulder;
        internal BonePose LeftUpperArm;
        internal BonePose RightUpperArm;
        internal BonePose LeftLowerArm;
        internal BonePose RightLowerArm;
        internal BonePose LeftHand;
        internal BonePose RightHand;

        public AvatarBones(
            BonePose hips, BonePose spine, BonePose chest, BonePose upperChest, BonePose neck, BonePose head,
            BonePose leftUpperLeg, BonePose rightUpperLeg, BonePose leftLowerLeg, BonePose rightLowerLeg,
            BonePose leftFoot, BonePose rightFoot,
            BonePose leftShoulder, BonePose rightShoulder, BonePose leftUpperArm, BonePose rightUpperArm,
            BonePose leftLowerArm, BonePose rightLowerArm, BonePose leftHand, BonePose rightHand)
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
