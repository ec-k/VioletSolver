using UnityEngine;

namespace VioletSolver.Pose
{
    // TODO: This class must have operator.
    internal struct AvatarPoseData
    {
        internal Quaternion this[HumanBodyBones boneName]
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

                    HumanBodyBones.LeftThumbProximal => LeftThumbProximal,
                    HumanBodyBones.LeftThumbIntermediate => LeftThumbIntermediate,
                    HumanBodyBones.LeftThumbDistal => LeftThumbDistal,
                    HumanBodyBones.LeftIndexProximal => LeftIndexProximal,
                    HumanBodyBones.LeftIndexIntermediate => LeftIndexIntermediate,
                    HumanBodyBones.LeftIndexDistal => LeftIndexDistal,
                    HumanBodyBones.LeftMiddleProximal => LeftMiddleProximal,
                    HumanBodyBones.LeftMiddleIntermediate => LeftMiddleIntermediate,
                    HumanBodyBones.LeftMiddleDistal => LeftMiddleDistal,
                    HumanBodyBones.LeftRingProximal => LeftRingProximal,
                    HumanBodyBones.LeftRingIntermediate => LeftRingIntermediate,
                    HumanBodyBones.LeftRingDistal => LeftRingDistal,
                    HumanBodyBones.LeftLittleProximal => LeftLittleProximal,
                    HumanBodyBones.LeftLittleIntermediate => LeftLittleIntermediate,
                    HumanBodyBones.LeftLittleDistal => LeftLittleDistal,

                    HumanBodyBones.RightThumbProximal => RightThumbProximal,
                    HumanBodyBones.RightThumbIntermediate => RightThumbIntermediate,
                    HumanBodyBones.RightThumbDistal => RightThumbDistal,
                    HumanBodyBones.RightIndexProximal => RightIndexProximal,
                    HumanBodyBones.RightIndexIntermediate => RightIndexIntermediate,
                    HumanBodyBones.RightIndexDistal => RightIndexDistal,
                    HumanBodyBones.RightMiddleProximal => RightMiddleProximal,
                    HumanBodyBones.RightMiddleIntermediate => RightMiddleIntermediate,
                    HumanBodyBones.RightMiddleDistal => RightMiddleDistal,
                    HumanBodyBones.RightRingProximal => RightRingProximal,
                    HumanBodyBones.RightRingIntermediate => RightRingIntermediate,
                    HumanBodyBones.RightRingDistal => RightRingDistal,
                    HumanBodyBones.RightLittleProximal => RightLittleProximal,
                    HumanBodyBones.RightLittleIntermediate => RightLittleIntermediate,
                    HumanBodyBones.RightLittleDistal => RightLittleDistal,

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

                    case HumanBodyBones.LeftThumbProximal: LeftThumbProximal = value; return;
                    case HumanBodyBones.LeftThumbIntermediate: LeftThumbIntermediate = value; return;
                    case HumanBodyBones.LeftThumbDistal: LeftThumbDistal = value; return;
                    case HumanBodyBones.LeftIndexProximal: LeftIndexProximal = value; return;
                    case HumanBodyBones.LeftIndexIntermediate: LeftIndexIntermediate = value; return;
                    case HumanBodyBones.LeftIndexDistal: LeftIndexDistal = value; return;
                    case HumanBodyBones.LeftMiddleProximal: LeftMiddleProximal = value; return;
                    case HumanBodyBones.LeftMiddleIntermediate: LeftMiddleIntermediate = value; return;
                    case HumanBodyBones.LeftMiddleDistal: LeftMiddleDistal = value; return;
                    case HumanBodyBones.LeftRingProximal: LeftRingProximal = value; return;
                    case HumanBodyBones.LeftRingIntermediate: LeftRingIntermediate = value; return;
                    case HumanBodyBones.LeftRingDistal: LeftRingDistal = value; return;
                    case HumanBodyBones.LeftLittleProximal: LeftLittleProximal = value; return;
                    case HumanBodyBones.LeftLittleIntermediate: LeftLittleIntermediate = value; return;
                    case HumanBodyBones.LeftLittleDistal: LeftLittleDistal = value; return;

                    case HumanBodyBones.RightThumbProximal: RightThumbProximal = value; return;
                    case HumanBodyBones.RightThumbIntermediate: RightThumbIntermediate = value; return;
                    case HumanBodyBones.RightThumbDistal: RightThumbDistal = value; return;
                    case HumanBodyBones.RightIndexProximal: RightIndexProximal = value; return;
                    case HumanBodyBones.RightIndexIntermediate: RightIndexIntermediate = value; return;
                    case HumanBodyBones.RightIndexDistal: RightIndexDistal = value; return;
                    case HumanBodyBones.RightMiddleProximal: RightMiddleProximal = value; return;
                    case HumanBodyBones.RightMiddleIntermediate: RightMiddleIntermediate = value; return;
                    case HumanBodyBones.RightMiddleDistal: RightMiddleDistal = value; return;
                    case HumanBodyBones.RightRingProximal: RightRingProximal = value; return;
                    case HumanBodyBones.RightRingIntermediate: RightRingIntermediate = value; return;
                    case HumanBodyBones.RightRingDistal: RightRingDistal = value; return;
                    case HumanBodyBones.RightLittleProximal: RightLittleProximal = value; return;
                    case HumanBodyBones.RightLittleIntermediate: RightLittleIntermediate = value; return;
                    case HumanBodyBones.RightLittleDistal: RightLittleDistal = value; return;

                    case HumanBodyBones.LeftEye: LeftEye = value; return;
                    case HumanBodyBones.RightEye: RightEye = value; return;
                }
            }
        }

        internal float time;

        // Ik Target Positions
        internal Vector3 HipsPosition;
        internal Vector3 HeadPosition;

        internal Vector3 LeftShoulderPosition;
        internal Vector3 LeftElbowPosition;
        internal Vector3 LeftHandPosition;

        internal Vector3 RightShoulderPosition;
        internal Vector3 RightElbowPosition;
        internal Vector3 RightHandPosition;

        internal Vector3 LeftThighPosition;
        internal Vector3 LeftKneePosition;
        internal Vector3 LeftFootPosition;

        internal Vector3 RightThighPosition;
        internal Vector3 RightKneePosition;
        internal Vector3 RightFootPosition;

        // Bone Rotations in solving without IK
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

        internal Quaternion LeftThumbProximal;
        internal Quaternion LeftThumbIntermediate;
        internal Quaternion LeftThumbDistal;
        internal Quaternion LeftIndexProximal;
        internal Quaternion LeftIndexIntermediate;
        internal Quaternion LeftIndexDistal;
        internal Quaternion LeftMiddleProximal;
        internal Quaternion LeftMiddleIntermediate;
        internal Quaternion LeftMiddleDistal;
        internal Quaternion LeftRingProximal;
        internal Quaternion LeftRingIntermediate;
        internal Quaternion LeftRingDistal;
        internal Quaternion LeftLittleProximal;
        internal Quaternion LeftLittleIntermediate;
        internal Quaternion LeftLittleDistal;

        internal Quaternion RightThumbProximal;
        internal Quaternion RightThumbIntermediate;
        internal Quaternion RightThumbDistal;
        internal Quaternion RightIndexProximal;
        internal Quaternion RightIndexIntermediate;
        internal Quaternion RightIndexDistal;
        internal Quaternion RightMiddleProximal;
        internal Quaternion RightMiddleIntermediate;
        internal Quaternion RightMiddleDistal;
        internal Quaternion RightRingProximal;
        internal Quaternion RightRingIntermediate;
        internal Quaternion RightRingDistal;
        internal Quaternion RightLittleProximal;
        internal Quaternion RightLittleIntermediate;
        internal Quaternion RightLittleDistal;

        internal Quaternion LeftEye;
        internal Quaternion RightEye;


        internal void SetLeftHandData(HandData handData)
        {
            LeftHand = handData.Wrist;
            LeftThumbProximal = handData.ThumbCMC;
            LeftThumbIntermediate = handData.ThumbMCP;
            LeftThumbDistal = handData.ThumbIP;
            LeftIndexProximal = handData.IndexFingerMCP;
            LeftIndexIntermediate = handData.IndexFingerPIP;
            LeftIndexDistal = handData.IndexFingerDIP;
            LeftMiddleProximal = handData.MiddleFingerMCP;
            LeftMiddleIntermediate = handData.MiddleFingerPIP;
            LeftMiddleDistal = handData.MiddleFingerDIP;
            LeftRingProximal = handData.RingFingerMCP;
            LeftRingIntermediate = handData.RingFingerPIP;
            LeftRingDistal = handData.RingFingerDIP;
            LeftLittleProximal = handData.PinkyMCP;
            LeftLittleIntermediate = handData.PinkyPIP;
            LeftLittleDistal = handData.PinkyDIP;
        }
        internal void SetRightHandData(HandData handData)
        {
            RightHand = handData.Wrist;
            RightThumbProximal = handData.ThumbCMC;
            RightThumbIntermediate = handData.ThumbMCP;
            RightThumbDistal = handData.ThumbIP;
            RightIndexProximal = handData.IndexFingerMCP;
            RightIndexIntermediate = handData.IndexFingerPIP;
            RightIndexDistal = handData.IndexFingerDIP;
            RightMiddleProximal = handData.MiddleFingerMCP;
            RightMiddleIntermediate = handData.MiddleFingerPIP;
            RightMiddleDistal = handData.MiddleFingerDIP;
            RightRingProximal = handData.RingFingerMCP;
            RightRingIntermediate = handData.RingFingerPIP;
            RightRingDistal = handData.RingFingerDIP;
            RightLittleProximal = handData.PinkyMCP;
            RightLittleIntermediate = handData.PinkyPIP;
            RightLittleDistal = handData.PinkyDIP;
        }
    }
}
