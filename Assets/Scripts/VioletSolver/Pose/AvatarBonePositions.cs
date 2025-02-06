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
    }
}
