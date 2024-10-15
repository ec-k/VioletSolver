namespace VioletSolver
{
    public class AvatarPoseSolver
    {
        public AvatarPoseSolver() { }
        // Probably, this class has PoseSolver, HandSolver and FaceSolver.
        // Use them.Solve in a function below and solve pose holisticly.
        public static AvatarPoseData Solve(IHolisticLandmarks landmarks, AvatarBonePositions restBonePositions)
        {
            var solvedPose = PoseSolver.SolvePose(landmarks.Pose.Landmarks, restBonePositions);
            solvedPose.Neck = FaceResolver.Solve(landmarks.Face.Landmarks);
            return solvedPose;
        }
        public static AvatarPoseData Solve(ILandmarks landmarks, AvatarBonePositions restBonePositions)
        {
            var solvedPose = PoseSolver.SolvePose(landmarks.Landmarks, restBonePositions);
            return solvedPose;
        }
    }
}
