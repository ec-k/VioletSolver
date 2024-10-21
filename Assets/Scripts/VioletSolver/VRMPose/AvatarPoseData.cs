using UnityEngine;

namespace VioletSolver
{
    // TODO: This class must have operator.
    public struct AvatarPoseData
    {
        public Quaternion this[HumanBodyBones boneName]
        {
            get
            {
                return boneName switch
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

                    HumanBodyBones.LeftEye => LeftEye,
                    HumanBodyBones.RightEye => RightEye,

                    _ => Quaternion.identity
                };
            }
            set
            {
                switch (boneName)
                {
                    case HumanBodyBones.Hips: Hips = value; return;
                    case HumanBodyBones.Spine: Spine = value; return;
                    case HumanBodyBones.Chest: Chest = value; return;
                    case HumanBodyBones.UpperChest: UpperChest = value; return;
                    case HumanBodyBones.Neck: Neck = value; return;
                    case HumanBodyBones.Head: Head = value; return;

                    case HumanBodyBones.LeftUpperLeg: LeftUpperLeg = value; return;
                    case HumanBodyBones.RightUpperLeg: RightUpperLeg = value; return;
                    case HumanBodyBones.LeftLowerLeg: LeftLowerLeg = value; return;
                    case HumanBodyBones.RightLowerLeg: RightLowerLeg = value; return;
                    case HumanBodyBones.LeftFoot: LeftFoot = value; return;
                    case HumanBodyBones.RightFoot: RightFoot = value; return;

                    case HumanBodyBones.LeftShoulder: LeftShoulder = value; return;
                    case HumanBodyBones.RightShoulder: RightShoulder = value; return;
                    case HumanBodyBones.LeftUpperArm: LeftUpperArm = value; return;
                    case HumanBodyBones.RightUpperArm: RightUpperArm = value; return;
                    case HumanBodyBones.LeftLowerArm: LeftLowerArm = value; return;
                    case HumanBodyBones.RightLowerArm: RightLowerArm = value; return;
                    case HumanBodyBones.LeftHand: LeftHand = value; return;
                    case HumanBodyBones.RightHand: RightHand = value; return;

                    case HumanBodyBones.LeftEye: LeftEye = value; return;
                    case HumanBodyBones.RightEye: RightEye = value; return;
                }
            }
        }

        public float time;

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

        public Quaternion LeftEye;
        public Quaternion RightEye;
    }
}
