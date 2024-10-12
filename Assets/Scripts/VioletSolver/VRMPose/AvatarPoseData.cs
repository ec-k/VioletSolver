using UnityEngine;

namespace VioletSolver
{
    // TODO: This class must have operator.
    public struct AvatarPoseData
    {
        // I think this must be remade as a get property.
        public Quaternion BodyBones(HumanBodyBones boneName) => boneName switch
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

        public Vector3 HipsPosition;

        public Quaternion Hips;
        public Quaternion Spine;
        public Quaternion Chest;
        public Quaternion UpperChest;
        public Quaternion Neck;
        public Quaternion Head;

        public Quaternion LeftUpperLeg;
        public Quaternion RightUpperLeg;
        public Quaternion LeftLowerLeg;
        public Quaternion RightLowerLeg;
        public Quaternion LeftFoot;
        public Quaternion RightFoot;

        public Quaternion LeftShoulder;
        public Quaternion RightShoulder;
        public Quaternion LeftUpperArm;
        public Quaternion RightUpperArm;
        public Quaternion LeftLowerArm;
        public Quaternion RightLowerArm;
        public Quaternion LeftHand;
        public Quaternion RightHand;
    }
}
