using UnityEngine;
using VioletSolver.Pose;
namespace VioletSolver.Development
{
    internal class PoseInterpolator
    {
        private AvatarPoseData _prevPoseData;
        private AvatarPoseData _nextPoseData;
        private float _lastProcessedTime;
        private const float _dataInterval = 1f / 30f;

        public PoseInterpolator()
        {
            _prevPoseData = new AvatarPoseData();
            _nextPoseData = new AvatarPoseData();
            _lastProcessedTime = 0f;
        }

        public AvatarPoseData UpdateAndInterpolate(AvatarPoseData newInputPose)
        {
            if (newInputPose.time > _lastProcessedTime)
            {
                _prevPoseData = _nextPoseData;
                _nextPoseData = newInputPose;
                _lastProcessedTime = newInputPose.time;
            }

            float interpolationAmount = Mathf.Clamp01((Time.time - _lastProcessedTime) / _dataInterval);

            AvatarPoseData result = new AvatarPoseData();

            result.time = newInputPose.time;

            // Hips Position
            result.HipsPosition = Vector3.Lerp(_prevPoseData.HipsPosition, _nextPoseData.HipsPosition, interpolationAmount);
            result.HeadPosition = Vector3.Lerp(_prevPoseData.HeadPosition, _nextPoseData.HeadPosition, interpolationAmount);

            // Left Arm IK Targets
            result.LeftShoulderPosition = Vector3.Lerp(_prevPoseData.LeftShoulderPosition, _nextPoseData.LeftShoulderPosition, interpolationAmount);
            result.LeftElbowPosition = Vector3.Lerp(_prevPoseData.LeftElbowPosition, _nextPoseData.LeftElbowPosition, interpolationAmount);
            result.LeftHandPosition = Vector3.Lerp(_prevPoseData.LeftHandPosition, _nextPoseData.LeftHandPosition, interpolationAmount);

            // Right Arm IK Targets
            result.RightShoulderPosition = Vector3.Lerp(_prevPoseData.RightShoulderPosition, _nextPoseData.RightShoulderPosition, interpolationAmount);
            result.RightElbowPosition = Vector3.Lerp(_prevPoseData.RightElbowPosition, _nextPoseData.RightElbowPosition, interpolationAmount);
            result.RightHandPosition = Vector3.Lerp(_prevPoseData.RightHandPosition, _nextPoseData.RightHandPosition, interpolationAmount);

            // Left Leg IK Targets
            result.LeftThighPosition = Vector3.Lerp(_prevPoseData.LeftThighPosition, _nextPoseData.LeftThighPosition, interpolationAmount);
            result.LeftKneePosition = Vector3.Lerp(_prevPoseData.LeftKneePosition, _nextPoseData.LeftKneePosition, interpolationAmount);
            result.LeftFootPosition = Vector3.Lerp(_prevPoseData.LeftFootPosition, _nextPoseData.LeftFootPosition, interpolationAmount);

            // Right Leg IK Targets
            result.RightThighPosition = Vector3.Lerp(_prevPoseData.RightThighPosition, _nextPoseData.RightThighPosition, interpolationAmount);
            result.RightKneePosition = Vector3.Lerp(_prevPoseData.RightKneePosition, _nextPoseData.RightKneePosition, interpolationAmount);
            result.RightFootPosition = Vector3.Lerp(_prevPoseData.RightFootPosition, _nextPoseData.RightFootPosition, interpolationAmount);

            // Bone Rotations
            result.Hips = Quaternion.Slerp(_prevPoseData.Hips, _nextPoseData.Hips, interpolationAmount);
            result.Spine = Quaternion.Slerp(_prevPoseData.Spine, _nextPoseData.Spine, interpolationAmount);
            result.Chest = Quaternion.Slerp(_prevPoseData.Chest, _nextPoseData.Chest, interpolationAmount);
            result.UpperChest = Quaternion.Slerp(_prevPoseData.UpperChest, _nextPoseData.UpperChest, interpolationAmount);
            result.Neck = Quaternion.Slerp(_prevPoseData.Neck, _nextPoseData.Neck, interpolationAmount);
            result.Head = Quaternion.Slerp(_prevPoseData.Head, _nextPoseData.Head, interpolationAmount);

            result.LeftUpperLeg = Quaternion.Slerp(_prevPoseData.LeftUpperLeg, _nextPoseData.LeftUpperLeg, interpolationAmount);
            result.RightUpperLeg = Quaternion.Slerp(_prevPoseData.RightUpperLeg, _nextPoseData.RightUpperLeg, interpolationAmount);
            result.LeftLowerLeg = Quaternion.Slerp(_prevPoseData.LeftLowerLeg, _nextPoseData.LeftLowerLeg, interpolationAmount);
            result.RightLowerLeg = Quaternion.Slerp(_prevPoseData.RightLowerLeg, _nextPoseData.RightLowerLeg, interpolationAmount);
            result.LeftFoot = Quaternion.Slerp(_prevPoseData.LeftFoot, _nextPoseData.LeftFoot, interpolationAmount);
            result.RightFoot = Quaternion.Slerp(_prevPoseData.RightFoot, _nextPoseData.RightFoot, interpolationAmount);

            result.LeftShoulder = Quaternion.Slerp(_prevPoseData.LeftShoulder, _nextPoseData.LeftShoulder, interpolationAmount);
            result.RightShoulder = Quaternion.Slerp(_prevPoseData.RightShoulder, _nextPoseData.RightShoulder, interpolationAmount);
            result.LeftUpperArm = Quaternion.Slerp(_prevPoseData.LeftUpperArm, _nextPoseData.LeftUpperArm, interpolationAmount);
            result.RightUpperArm = Quaternion.Slerp(_prevPoseData.RightUpperArm, _nextPoseData.RightUpperArm, interpolationAmount);
            result.LeftLowerArm = Quaternion.Slerp(_prevPoseData.LeftLowerArm, _nextPoseData.LeftLowerArm, interpolationAmount);
            result.RightLowerArm = Quaternion.Slerp(_prevPoseData.RightLowerArm, _nextPoseData.RightLowerArm, interpolationAmount);
            result.LeftHand = Quaternion.Slerp(_prevPoseData.LeftHand, _nextPoseData.LeftHand, interpolationAmount);
            result.RightHand = Quaternion.Slerp(_prevPoseData.RightHand, _nextPoseData.RightHand, interpolationAmount);

            result.LeftThumbProximal = Quaternion.Slerp(_prevPoseData.LeftThumbProximal, _nextPoseData.LeftThumbProximal, interpolationAmount);
            result.LeftThumbIntermediate = Quaternion.Slerp(_prevPoseData.LeftThumbIntermediate, _nextPoseData.LeftThumbIntermediate, interpolationAmount);
            result.LeftThumbDistal = Quaternion.Slerp(_prevPoseData.LeftThumbDistal, _nextPoseData.LeftThumbDistal, interpolationAmount);
            result.LeftIndexProximal = Quaternion.Slerp(_prevPoseData.LeftIndexProximal, _nextPoseData.LeftIndexProximal, interpolationAmount);
            result.LeftIndexIntermediate = Quaternion.Slerp(_prevPoseData.LeftIndexIntermediate, _nextPoseData.LeftIndexIntermediate, interpolationAmount);
            result.LeftIndexDistal = Quaternion.Slerp(_prevPoseData.LeftIndexDistal, _nextPoseData.LeftIndexDistal, interpolationAmount);
            result.LeftMiddleProximal = Quaternion.Slerp(_prevPoseData.LeftMiddleProximal, _nextPoseData.LeftMiddleProximal, interpolationAmount);
            result.LeftMiddleIntermediate = Quaternion.Slerp(_prevPoseData.LeftMiddleIntermediate, _nextPoseData.LeftMiddleIntermediate, interpolationAmount);
            result.LeftMiddleDistal = Quaternion.Slerp(_prevPoseData.LeftMiddleDistal, _nextPoseData.LeftMiddleDistal, interpolationAmount);
            result.LeftRingProximal = Quaternion.Slerp(_prevPoseData.LeftRingProximal, _nextPoseData.LeftRingProximal, interpolationAmount);
            result.LeftRingIntermediate = Quaternion.Slerp(_prevPoseData.LeftRingIntermediate, _nextPoseData.LeftRingIntermediate, interpolationAmount);
            result.LeftRingDistal = Quaternion.Slerp(_prevPoseData.LeftRingDistal, _nextPoseData.LeftRingDistal, interpolationAmount);
            result.LeftLittleProximal = Quaternion.Slerp(_prevPoseData.LeftLittleProximal, _nextPoseData.LeftLittleProximal, interpolationAmount);
            result.LeftLittleIntermediate = Quaternion.Slerp(_prevPoseData.LeftLittleIntermediate, _nextPoseData.LeftLittleIntermediate, interpolationAmount);
            result.LeftLittleDistal = Quaternion.Slerp(_prevPoseData.LeftLittleDistal, _nextPoseData.LeftLittleDistal, interpolationAmount);

            result.RightThumbProximal = Quaternion.Slerp(_prevPoseData.RightThumbProximal, _nextPoseData.RightThumbProximal, interpolationAmount);
            result.RightThumbIntermediate = Quaternion.Slerp(_prevPoseData.RightThumbIntermediate, _nextPoseData.RightThumbIntermediate, interpolationAmount);
            result.RightThumbDistal = Quaternion.Slerp(_prevPoseData.RightThumbDistal, _nextPoseData.RightThumbDistal, interpolationAmount);
            result.RightIndexProximal = Quaternion.Slerp(_prevPoseData.RightIndexProximal, _nextPoseData.RightIndexProximal, interpolationAmount);
            result.RightIndexIntermediate = Quaternion.Slerp(_prevPoseData.RightIndexIntermediate, _nextPoseData.RightIndexIntermediate, interpolationAmount);
            result.RightIndexDistal = Quaternion.Slerp(_prevPoseData.RightIndexDistal, _nextPoseData.RightIndexDistal, interpolationAmount);
            result.RightMiddleProximal = Quaternion.Slerp(_prevPoseData.RightMiddleProximal, _nextPoseData.RightMiddleProximal, interpolationAmount);
            result.RightMiddleIntermediate = Quaternion.Slerp(_prevPoseData.RightMiddleIntermediate, _nextPoseData.RightMiddleIntermediate, interpolationAmount);
            result.RightMiddleDistal = Quaternion.Slerp(_prevPoseData.RightMiddleDistal, _nextPoseData.RightMiddleDistal, interpolationAmount);
            result.RightRingProximal = Quaternion.Slerp(_prevPoseData.RightRingProximal, _nextPoseData.RightRingProximal, interpolationAmount);
            result.RightRingIntermediate = Quaternion.Slerp(_prevPoseData.RightRingIntermediate, _nextPoseData.RightRingIntermediate, interpolationAmount);
            result.RightRingDistal = Quaternion.Slerp(_prevPoseData.RightRingDistal, _nextPoseData.RightRingDistal, interpolationAmount);
            result.RightLittleProximal = Quaternion.Slerp(_prevPoseData.RightLittleProximal, _nextPoseData.RightLittleProximal, interpolationAmount);
            result.RightLittleIntermediate = Quaternion.Slerp(_prevPoseData.RightLittleIntermediate, _nextPoseData.RightLittleIntermediate, interpolationAmount);
            result.RightLittleDistal = Quaternion.Slerp(_prevPoseData.RightLittleDistal, _nextPoseData.RightLittleDistal, interpolationAmount);

            result.LeftEye = Quaternion.Slerp(_prevPoseData.LeftEye, _nextPoseData.LeftEye, interpolationAmount);
            result.RightEye = Quaternion.Slerp(_prevPoseData.RightEye, _nextPoseData.RightEye, interpolationAmount);

            return result;
        }
    }
}