using UnityEngine;

namespace VioletSolver.Pose
{
    // TODO: This class must have operator.
    public struct AvatarBonePositions
    {
        public Vector3 this[HumanBodyBones index]
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
            set
            {
                switch (index)
                {
                    case HumanBodyBones.Hips : Hips = value; return;
                    case HumanBodyBones.Spine : Spine = value; return;
                    case HumanBodyBones.Chest : Chest = value; return;
                    case HumanBodyBones.UpperChest : UpperChest = value; return;
                    case HumanBodyBones.Neck : Neck = value; return;
                    case HumanBodyBones.Head : Head = value; return;
                    case HumanBodyBones.LeftUpperLeg : LeftUpperLeg = value; return;
                    case HumanBodyBones.RightUpperLeg : RightUpperLeg = value; return;
                    case HumanBodyBones.LeftLowerLeg : LeftLowerLeg = value; return;
                    case HumanBodyBones.RightLowerLeg : RightLowerLeg = value; return;
                    case HumanBodyBones.LeftFoot : LeftFoot = value; return;
                    case HumanBodyBones.RightFoot : RightFoot = value; return;
                    case HumanBodyBones.LeftShoulder : LeftShoulder = value; return;
                    case HumanBodyBones.RightShoulder : RightShoulder = value; return;
                    case HumanBodyBones.LeftUpperArm : LeftUpperArm = value; return;
                    case HumanBodyBones.RightUpperArm : RightUpperArm = value; return;
                    case HumanBodyBones.LeftLowerArm : LeftLowerArm = value; return;
                    case HumanBodyBones.RightLowerArm : RightLowerArm = value; return;
                    case HumanBodyBones.LeftHand : LeftHand = value; return;
                    case HumanBodyBones.RightHand: RightHand = value; return;
                }
            }
        }

        public Vector3 Hips;
        public Vector3 Spine;
        public Vector3 Chest;
        public Vector3 UpperChest;
        public Vector3 Neck;
        public Vector3 Head;

        public Vector3 LeftUpperLeg;
        public Vector3 RightUpperLeg;
        public Vector3 LeftLowerLeg;
        public Vector3 RightLowerLeg;
        public Vector3 LeftFoot;
        public Vector3 RightFoot;

        public Vector3 LeftShoulder;
        public Vector3 RightShoulder;
        public Vector3 LeftUpperArm;
        public Vector3 RightUpperArm;
        public Vector3 LeftLowerArm;
        public Vector3 RightLowerArm;
        public Vector3 LeftHand;
        public Vector3 RightHand;
    }
}
