using UnityEngine;

namespace VioletSolver
{
    public struct AvatarPoseData
    {
        public Quaternion BodyBones(HumanBodyBones boneName) => boneName switch
        {
            HumanBodyBones.Hips => Hips.Rotation,
            HumanBodyBones.Spine => Spine.Rotation,
            HumanBodyBones.Chest => Chest.Rotation,
            HumanBodyBones.UpperChest => UpperChest.Rotation,
            HumanBodyBones.Neck => Neck.Rotation,
            HumanBodyBones.Head => Head.Rotation,

            HumanBodyBones.LeftUpperLeg => LeftUpperLeg.Rotation,
            HumanBodyBones.RightUpperLeg => RightUpperLeg.Rotation,
            HumanBodyBones.LeftLowerLeg => LeftLowerLeg.Rotation,
            HumanBodyBones.RightLowerLeg => RightLowerLeg.Rotation,
            HumanBodyBones.LeftFoot => LeftFoot.Rotation,
            HumanBodyBones.RightFoot => RightFoot.Rotation,

            HumanBodyBones.LeftShoulder => LeftShoulder.Rotation,
            HumanBodyBones.RightShoulder => RightShoulder.Rotation,
            HumanBodyBones.LeftUpperArm => LeftUpperArm.Rotation,
            HumanBodyBones.RightUpperArm => RightUpperArm.Rotation,
            HumanBodyBones.LeftLowerArm => LeftLowerArm.Rotation,
            HumanBodyBones.RightLowerArm => RightLowerArm.Rotation,
            HumanBodyBones.LeftHand => LeftHand.Rotation,
            HumanBodyBones.RightHand => RightHand.Rotation,

            _ => Quaternion.identity
        };

        public BoneRotation Hips;
        public BoneRotation Spine;
        public BoneRotation Chest;
        public BoneRotation UpperChest;
        public BoneRotation Neck;
        public BoneRotation Head;

        public BoneRotation LeftUpperLeg;
        public BoneRotation RightUpperLeg;
        public BoneRotation LeftLowerLeg;
        public BoneRotation RightLowerLeg;
        public BoneRotation LeftFoot;
        public BoneRotation RightFoot;

        public BoneRotation LeftShoulder;
        public BoneRotation RightShoulder;
        public BoneRotation LeftUpperArm;
        public BoneRotation RightUpperArm;
        public BoneRotation LeftLowerArm;
        public BoneRotation RightLowerArm;
        public BoneRotation LeftHand;
        public BoneRotation RightHand;
    }

    public struct BoneRotation
    {
        Quaternion _rotation;
        HumanBodyBones _boneName;

        public Quaternion Rotation => _rotation;
        public HumanBodyBones BoneName => _boneName;

        void Update(Quaternion rotation)
        {
            _rotation = rotation;
        }
    }
}
