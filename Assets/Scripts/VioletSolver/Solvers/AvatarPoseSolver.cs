using System.Collections.Generic;
using UnityEngine;
using VRM;

using MediaPipeBlendshapes = HolisticPose.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver
{
    public static class AvatarPoseSolver
    {
        // Probably, this class has PoseSolver, HandSolver and FaceSolver.
        // Use them.Solve in a function below and solve pose holisticly.
        public static AvatarPoseData Solve(IHolisticLandmarks landmarks, AvatarBonePositions restBonePositions)
        {
            var solvedPose = new AvatarPoseData();

            if(ExistLandmarks(landmarks.Pose))
                solvedPose = PoseSolver.SolvePose(landmarks.Pose.Landmarks, restBonePositions);
            if(ExistLandmarks(landmarks.Face))
                solvedPose.Neck = FaceResolver.Solve(landmarks.Face.Landmarks);
            if(ExistLandmarks(landmarks.LeftHand))
                solvedPose.SetLeftHandData(HandResolver.SolveLeftHand(landmarks.LeftHand));
            if (ExistLandmarks(landmarks.RightHand))
                solvedPose.SetRightHandData(HandResolver.SolveRightHand(landmarks.RightHand));
            
            solvedPose.time = landmarks.Pose.Time;

            return solvedPose;
        }

        public static (Dictionary<BlendShapePreset, float>, Quaternion, Quaternion) Solve(Dictionary<MediaPipeBlendshapes, float> weights)
        {
            var blendshapes = FaceResolver.SolveFacialExpression(weights);
            var (leftEyeRotation, rightEyeRotation) = FaceResolver.SolveEye(weights);
            return (blendshapes, leftEyeRotation, rightEyeRotation);
        }

        /// <summary>
        ///     Solve face for Perfect Sync.
        /// </summary>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static (Dictionary<MediaPipeBlendshapes, float>, Quaternion, Quaternion) SolvePerfectly(Dictionary<MediaPipeBlendshapes, float> weights)
        {
            var blendshapes = FaceResolver.SolveFacialExpressionPerfectly(weights);
            var (leftEyeRotation, rightEyeRotation) = FaceResolver.SolveEye(weights);
            return (blendshapes, leftEyeRotation, rightEyeRotation);
        }

        static bool ExistLandmarks(ILandmarks landmarks)
            => landmarks != null
                && landmarks.Landmarks != null
                && landmarks.Landmarks.Count > 0;
    }
}
