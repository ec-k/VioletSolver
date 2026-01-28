using UnityEngine;

namespace VioletSolver.Pose
{
    internal class AvatarBoneRotations
    {
        internal Quaternion this[HumanBodyBones index]
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

                    _ => Quaternion.identity
                };
            }
        }

        internal Quaternion Hips;
        internal Quaternion Spine;
        internal Quaternion Chest;
        internal Quaternion UpperChest;
        internal Quaternion Neck;
        internal Quaternion Head;

        internal Quaternion LeftUpperLeg;
        internal Quaternion RightUpperLeg;
        internal Quaternion LeftLowerLeg;
        internal Quaternion RightLowerLeg;
        internal Quaternion LeftFoot;
        internal Quaternion RightFoot;

        internal Quaternion LeftShoulder;
        internal Quaternion RightShoulder;
        internal Quaternion LeftUpperArm;
        internal Quaternion RightUpperArm;
        internal Quaternion LeftLowerArm;
        internal Quaternion RightLowerArm;
        internal Quaternion LeftHand;
        internal Quaternion RightHand;

        public AvatarBoneRotations(
            Quaternion hips, Quaternion spine, Quaternion chest, Quaternion upperChest, Quaternion neck, Quaternion head,
            Quaternion leftUpperLeg, Quaternion rightUpperLeg, Quaternion leftLowerLeg, Quaternion rightLowerLeg,
            Quaternion leftFoot, Quaternion rightFoot,
            Quaternion leftShoulder, Quaternion rightShoulder, Quaternion leftUpperArm, Quaternion rightUpperArm,
            Quaternion leftLowerArm, Quaternion rightLowerArm, Quaternion leftHand, Quaternion rightHand)
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
