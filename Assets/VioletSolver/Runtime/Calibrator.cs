using UnityEngine;
using VioletSolver.Landmarks;
using poseIndex = HumanLandmarks.PoseLandmarks.Types.LandmarkIndex;

namespace VioletSolver
{
    public class Calibrator
    {
        public void Calibrate()
        {

        }

        /// <summary></summary>
        /// <param name="animator"></param>
        /// <param name="poseLandmarks"></param>
        /// <returns>user to avatar arm length ratio</returns>
        float CalculateArmLengthRatio(in Animator animator, in ILandmarks poseLandmarks)
        {
            var (leftArmLengthRatio, rightArmLengthRatio) = (1f, 1f);
            // about LeftArm
            {
                var userShoulder = poseLandmarks.Landmarks[(int)poseIndex.LeftShoulder];
                var userElbow = poseLandmarks.Landmarks[(int)poseIndex.LeftElbow];
                var userWrist = poseLandmarks.Landmarks[(int)poseIndex.LeftWrist];

                var userArmLength = (userWrist - userElbow).Position.magnitude + (userElbow - userShoulder).Position.magnitude;

                var avatarShoulder = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position;
                var avatarElbow = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).position;
                var avatarWrist = animator.GetBoneTransform(HumanBodyBones.LeftHand).position;

                var avatarArmLength = (avatarWrist - avatarElbow).magnitude + (avatarElbow - avatarShoulder).magnitude;

                leftArmLengthRatio = avatarArmLength / userArmLength;
            }
            // about RightArm
            {
                var userShoulder = poseLandmarks.Landmarks[(int)poseIndex.RightShoulder];
                var userElbow = poseLandmarks.Landmarks[(int)poseIndex.RightElbow];
                var userWrist = poseLandmarks.Landmarks[(int)poseIndex.RightWrist];

                var userArmLength = (userWrist - userElbow).Position.magnitude + (userElbow - userShoulder).Position.magnitude;

                var avatarShoulder = animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
                var avatarElbow = animator.GetBoneTransform(HumanBodyBones.RightLowerArm).position;
                var avatarWrist = animator.GetBoneTransform(HumanBodyBones.RightHand).position;

                var avatarArmLength = (avatarWrist - avatarElbow).magnitude + (avatarElbow - avatarShoulder).magnitude;

                leftArmLengthRatio = avatarArmLength / userArmLength;
            }

            float avgLengthRatio = (leftArmLengthRatio + rightArmLengthRatio) / 2f;
            return avgLengthRatio;
        }
    }
}
