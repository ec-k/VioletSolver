// This code is is basically microsoft/Azure-Kinect-Samples repository, specifically from the UnityBodyTracking sample with some minor modifications by EC-K.
// This code is licensed under the MIT License (https://github.com/microsoft/Azure-Kinect-Samples/blob/master/LICENSE).


using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

using JointId = HumanLandmarks.KinectPoseLandmarks.Types.LandmarkIndex;
using VioletSolver.LandmarkProviders;
using HumanLandmarks;


namespace VioletSolver.Development
{
    public class KinectBoneRotationApplier : MonoBehaviour
    {
        [SerializeField] LandmarkReceiver _landmarkReceiver;
        [SerializeField] Animator _animator;
        [SerializeField] Transform _characterRootTransform;

        Dictionary<JointId, Quaternion> _absoluteOffsetMap;
        readonly int _kinectJointIdCount = Enum.GetValues(typeof(JointId)).Length;

        static HumanBodyBones MapKinectJoint(JointId joint)
        {
            // https://docs.microsoft.com/en-us/azure/Kinect-dk/body-joints
            switch (joint)
            {
                case JointId.Pelvis: return HumanBodyBones.Hips;
                case JointId.SpineNaval: return HumanBodyBones.Spine;
                case JointId.SpineChest: return HumanBodyBones.Chest;
                case JointId.Neck: return HumanBodyBones.Neck;
                case JointId.Head: return HumanBodyBones.Head;
                case JointId.HipLeft: return HumanBodyBones.LeftUpperLeg;
                case JointId.KneeLeft: return HumanBodyBones.LeftLowerLeg;
                case JointId.AnkleLeft: return HumanBodyBones.LeftFoot;
                case JointId.FootLeft: return HumanBodyBones.LeftToes;
                case JointId.HipRight: return HumanBodyBones.RightUpperLeg;
                case JointId.KneeRight: return HumanBodyBones.RightLowerLeg;
                case JointId.AnkleRight: return HumanBodyBones.RightFoot;
                case JointId.FootRight: return HumanBodyBones.RightToes;
                case JointId.ClavicleLeft: return HumanBodyBones.LeftShoulder;
                case JointId.ShoulderLeft: return HumanBodyBones.LeftUpperArm;
                case JointId.ElbowLeft: return HumanBodyBones.LeftLowerArm;
                case JointId.WristLeft: return HumanBodyBones.LeftHand;
                case JointId.ClavicleRight: return HumanBodyBones.RightShoulder;
                case JointId.ShoulderRight: return HumanBodyBones.RightUpperArm;
                case JointId.ElbowRight: return HumanBodyBones.RightLowerArm;
                case JointId.WristRight: return HumanBodyBones.RightHand;
                default: return HumanBodyBones.LastBone;
            }
        }
        void Start()
        {
            _absoluteOffsetMap = new Dictionary<JointId, Quaternion>();
            for (int i = 0; i < _kinectJointIdCount; i++)
            {
                HumanBodyBones hbb = MapKinectJoint((JointId)i);
                if (hbb != HumanBodyBones.LastBone)
                {
                    Transform transform = _animator.GetBoneTransform(hbb);
                    Quaternion absOffset = GetSkeletonBone(_animator, transform.name).rotation;
                    // find the absolute offset for the T-pose
                    while (!ReferenceEquals(transform, _characterRootTransform))
                    {
                        transform = transform.parent;
                        absOffset = GetSkeletonBone(_animator, transform.name).rotation * absOffset;
                    }
                    _absoluteOffsetMap[(JointId)i] = absOffset;
                }
            }
        }

        static SkeletonBone GetSkeletonBone(Animator animator, string boneName)
        {
            int count = 0;
            StringBuilder cloneName = new StringBuilder(boneName);
            cloneName.Append("(Clone)");
            foreach (SkeletonBone sb in animator.avatar.humanDescription.skeleton)
            {
                if (sb.name == boneName || sb.name == cloneName.ToString())
                {
                    return animator.avatar.humanDescription.skeleton[count];
                }
                count++;
            }
            return new SkeletonBone { rotation = Quaternion.identity };
        }

        void OnEnable()
        {
            _landmarkReceiver.OnLandmarksReceived += AnimateAvatar;
        }

        void OnDisable()
        {
            _landmarkReceiver.OnLandmarksReceived -= AnimateAvatar;
        }

        void AnimateAvatar(HolisticLandmarks landmarks, float timestamp)
        {
            for (int j = 0; j < _kinectJointIdCount; j++)
            {
                if (MapKinectJoint((JointId)j) != HumanBodyBones.LastBone && _absoluteOffsetMap.ContainsKey((JointId)j))
                {
                    // get the absolute offset
                    var absOffset = _absoluteOffsetMap[(JointId)j];
                    var finalJoint = _animator.GetBoneTransform(MapKinectJoint((JointId)j));
                    var kinectJointRotation = landmarks.KinectPoseLandmarks.Landmarks[j].Rotation;
                    if (kinectJointRotation is null)
                        continue;

                    var jointQ = new Quaternion(
                        kinectJointRotation.X,
                        kinectJointRotation.Y,
                        kinectJointRotation.Z,
                        kinectJointRotation.W);

                    finalJoint.rotation = jointQ * absOffset;
                }
            }
        }

    }
}
