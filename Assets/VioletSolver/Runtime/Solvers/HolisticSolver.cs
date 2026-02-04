using UnityEngine;

using VioletSolver.Landmarks;
using VioletSolver.Pose;
using VioletSolver.Solvers.RestPose;
using VioletSolver.Solver.Face;

namespace VioletSolver.Solver
{
    internal static class HolisticSolver
    {
        // Probably, this class has PoseSolver, HandSolver and FaceSolver.
        // Use them.Solve in a function below and solve pose holisticly.
        internal static AvatarPoseData Solve(in HolisticLandmarks landmarks, in AvatarBones restBones, bool useIk, bool isKinectPose)
        {
            var solvedPose = new AvatarPoseData();

            if(ExistLandmarks(landmarks.KinectPose) && isKinectPose)
                solvedPose = KinectPoseSolver.SolvePose(landmarks.KinectPose.Landmarks, restBones, useIk);
            if(ExistLandmarks(landmarks.MediaPipePose) && !isKinectPose)
                solvedPose = MediaPipePoseSolver.SolvePose(landmarks.MediaPipePose.Landmarks, restBones, useIk);
            if (ExistLandmarks(landmarks.Face) && !isKinectPose)
                solvedPose.Neck = MediaPipeHeadRotationSolver.Solve(landmarks.Face.Landmarks);
            if(ExistLandmarks(landmarks.LeftHand))
                solvedPose.SetLeftHandData(HandSolver.SolveLeftHand(landmarks.LeftHand));
            if (ExistLandmarks(landmarks.RightHand))
                solvedPose.SetRightHandData(HandSolver.SolveRightHand(landmarks.RightHand));
            
            solvedPose.time = landmarks.Pose.Time;

            return solvedPose;
        }

        static bool ExistLandmarks(in ILandmarkList landmarks)
            => landmarks != null
                && landmarks.Landmarks != null
                && landmarks.Landmarks.Count > 0;
    }
}
