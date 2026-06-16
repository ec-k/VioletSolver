namespace VioletSolver
{
    /// <summary>
    /// Configuration passed to solvers for coordinate space conversion.
    /// </summary>
    public readonly struct SolverContext
    {
        /// <summary>Scale factor to convert pose landmark positions to avatar world space.</summary>
        public float PoseScale { get; }

        public SolverContext(float poseScale)
        {
            PoseScale = poseScale;
        }
    }
}
