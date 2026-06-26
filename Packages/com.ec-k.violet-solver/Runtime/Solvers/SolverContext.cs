namespace VioletSolver
{
    /// <summary>
    /// Configuration passed to solvers for coordinate space conversion.
    /// </summary>
    public readonly struct SolverContext
    {
        /// <summary>Scale factor to convert pose landmark positions to avatar world space.</summary>
        public float PoseScale { get; }

        /// <summary>Scale factor to convert MediaPipe.Hand relative vectors to Kinect world space.</summary>
        public float HandScaleFactor { get; }

        public SolverContext(float poseScale, float handScaleFactor = 1f)
        {
            PoseScale = poseScale;
            HandScaleFactor = handScaleFactor;
        }
    }
}
