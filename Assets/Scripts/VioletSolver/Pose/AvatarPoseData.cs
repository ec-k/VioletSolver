using UnityEngine;

namespace VioletSolver.Pose
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

        public float time;

        // Ik Target Positions
        public Vector3 HipsPosition;
        public Vector3 HeadPosition;

        public Vector3 LeftShoulderPosition;
        public Vector3 LeftElbowPosition;
        public Vector3 LeftHandPosition;

        public Vector3 RightShoulderPosition;
        public Vector3 RightElbowPosition;
        public Vector3 RightHandPosition;

        public Vector3 LeftThighPosition;
        public Vector3 LeftKneePosition;
        public Vector3 LeftFootPosition;

        public Vector3 RightThighPosition;
        public Vector3 RightKneePosition;
        public Vector3 RightFootPosition;

        // Bone Rotations in solving without IK
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

        public Quaternion LeftThumbProximal;
        public Quaternion LeftThumbIntermediate;
        public Quaternion LeftThumbDistal;
        public Quaternion LeftIndexProximal;
        public Quaternion LeftIndexIntermediate;
        public Quaternion LeftIndexDistal;
        public Quaternion LeftMiddleProximal;
        public Quaternion LeftMiddleIntermediate;
        public Quaternion LeftMiddleDistal;
        public Quaternion LeftRingProximal;
        public Quaternion LeftRingIntermediate;
        public Quaternion LeftRingDistal;
        public Quaternion LeftLittleProximal;
        public Quaternion LeftLittleIntermediate;
        public Quaternion LeftLittleDistal;

        public Quaternion RightThumbProximal;
        public Quaternion RightThumbIntermediate;
        public Quaternion RightThumbDistal;
        public Quaternion RightIndexProximal;
        public Quaternion RightIndexIntermediate;
        public Quaternion RightIndexDistal;
        public Quaternion RightMiddleProximal;
        public Quaternion RightMiddleIntermediate;
        public Quaternion RightMiddleDistal;
        public Quaternion RightRingProximal;
        public Quaternion RightRingIntermediate;
        public Quaternion RightRingDistal;
        public Quaternion RightLittleProximal;
        public Quaternion RightLittleIntermediate;
        public Quaternion RightLittleDistal;

        public Quaternion LeftEye;
        public Quaternion RightEye;


        public void SetLeftHandData(HandData handData)
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
        public void SetRightHandData(HandData handData)
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
