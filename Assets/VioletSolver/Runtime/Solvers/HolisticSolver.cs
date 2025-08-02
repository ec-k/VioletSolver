using System.Collections.Generic;
using UnityEngine;
using VRM;

using VioletSolver.Landmarks;
using VioletSolver.Pose;
using MediaPipeBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver.Solver
{
    internal static class HolisticSolver
    {
        // Probably, this class has PoseSolver, HandSolver and FaceSolver.
        // Use them.Solve in a function below and solve pose holisticly.
        internal static AvatarPoseData Solve(IHolisticLandmarks landmarks, AvatarBonePositions restBonePositions, bool useIk)
        {
            var solvedPose = new AvatarPoseData();

            if(ExistLandmarks(landmarks.Pose))
                solvedPose = KinectPoseSolver.SolvePose(landmarks.Pose.Landmarks, restBonePositions, useIk);
            if(ExistLandmarks(landmarks.Face))
                solvedPose.Neck = FaceSolver.Solve(landmarks.Face.Landmarks);
            if(ExistLandmarks(landmarks.LeftHand))
                solvedPose.SetLeftHandData(HandSolver.SolveLeftHand(landmarks.LeftHand));
            if (ExistLandmarks(landmarks.RightHand))
                solvedPose.SetRightHandData(HandSolver.SolveRightHand(landmarks.RightHand));
            
            solvedPose.time = landmarks.Pose.Time;

            return solvedPose;
        }

        internal static (Dictionary<BlendShapePreset, float>, Quaternion, Quaternion) Solve(Dictionary<MediaPipeBlendshapes, float> weights)
        {
            var blendshapes = FaceSolver.SolveFacialExpression(weights);
            var (leftEyeRotation, rightEyeRotation) = FaceSolver.SolveEye(weights);
            return (blendshapes, leftEyeRotation, rightEyeRotation);
        }

        /// <summary>
        ///     Solve face for Perfect Sync.
        /// </summary>
        /// <param name="weights"></param>
        /// <returns></returns>
        internal static (Dictionary<MediaPipeBlendshapes, float>, Quaternion, Quaternion) SolvePerfectly(Dictionary<MediaPipeBlendshapes, float> weights)
        {
            var blendshapes = FaceSolver.SolveFacialExpressionPerfectly(weights);
            var (leftEyeRotation, rightEyeRotation) = FaceSolver.SolveEye(weights);
            return (blendshapes, leftEyeRotation, rightEyeRotation);
        }

        static bool ExistLandmarks(ILandmarks landmarks)
            => landmarks != null
                && landmarks.Landmarks != null
                && landmarks.Landmarks.Count > 0;
    }
}
