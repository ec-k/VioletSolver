using UnityEngine;

using VioletSolver.Landmarks;
using VioletSolver.Pose;
using VioletSolver.Solvers.RestPose;
using VioletSolver.Solver.Face;

namespace VioletSolver.Solver
{
    internal static class HolisticSolver
    {
        internal static AvatarPoseData Solve(in HolisticLandmarks landmarks, in AvatarBones restBones, Animator animator, bool useIk, bool isKinectPose)
        {
            var solvedPose = new AvatarPoseData();

            if(ExistLandmarks(landmarks.KinectPose) && isKinectPose)
                solvedPose = KinectPoseSolver.SolvePose(landmarks.KinectPose.Landmarks, restBones, useIk);
            if(ExistLandmarks(landmarks.MediaPipePose) && !isKinectPose)
                solvedPose = MediaPipePoseSolver.SolvePose(landmarks.MediaPipePose.Landmarks, restBones, useIk);
            if (ExistLandmarks(landmarks.Face) && !isKinectPose)
                solvedPose.Neck = MediaPipeHeadRotationSolver.Solve(landmarks.Face.Landmarks);

            // Solve hand rotations and apply fingertip alignment for IK wrist position
            if (ExistLandmarks(landmarks.LeftHand))
            {
                var leftHandData = HandSolver.SolveLeftHand(landmarks.LeftHand);
                solvedPose.SetLeftHandData(leftHandData);

                if (useIk)
                {
                    var poseWristPos = solvedPose.LeftHandPosition;
                    var avatarWristToDistal = GetAvatarWristToDistalVector(animator, HumanBodyBones.LeftHand, HumanBodyBones.LeftIndexDistal);
                    solvedPose.LeftHandPosition = FingertipAlignmentSolver.SolveWristPosition(
                        landmarks.LeftHand,
                        poseWristPos,
                        avatarWristToDistal);
                }
            }

            if (ExistLandmarks(landmarks.RightHand))
            {
                var rightHandData = HandSolver.SolveRightHand(landmarks.RightHand);
                solvedPose.SetRightHandData(rightHandData);

                if (useIk)
                {
                    var poseWristPos = solvedPose.RightHandPosition;
                    var avatarWristToDistal = GetAvatarWristToDistalVector(animator, HumanBodyBones.RightHand, HumanBodyBones.RightIndexDistal);
                    solvedPose.RightHandPosition = FingertipAlignmentSolver.SolveWristPosition(
                        landmarks.RightHand,
                        poseWristPos,
                        avatarWristToDistal);
                }
            }

            solvedPose.time = landmarks.Pose.Time;

            return solvedPose;
        }

        /// <summary>
        /// Gets the current vector from wrist to index finger distal on the avatar.
        /// </summary>
        static Vector3 GetAvatarWristToDistalVector(Animator animator, HumanBodyBones wristBone, HumanBodyBones indexDistalBone)
        {
            var wristTransform = animator.GetBoneTransform(wristBone);
            var distalTransform = animator.GetBoneTransform(indexDistalBone);

            if (wristTransform == null || distalTransform == null)
                return Vector3.forward * 0.1f; // Fallback default

            var worldVector = distalTransform.position - wristTransform.position;

            // Convert from world space to camera/local space by removing avatar rotation.
            // This is necessary because FingertipAlignmentSolver calculates in camera space,
            // and mixing world-space vectors with camera-space positions causes incorrect
            // hand extension when the avatar has a rotation offset.
            return Quaternion.Inverse(animator.transform.rotation) * worldVector;
        }

        static bool ExistLandmarks(in ILandmarkList landmarks)
            => landmarks != null
                && landmarks.Landmarks != null
                && landmarks.Landmarks.Count > 0;
    }
}
