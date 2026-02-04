using System.Collections.Generic;
using VioletSolver.Solver;
using MediaPipeBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver.Solver.Face
{
    /// <summary>
    /// Solver that applies MediaPipe blendshapes directly to the avatar using Perfect Sync.
    /// </summary>
    public class PerfectSyncBlendshapeSolver : IBlendshapeSolver
    {
        /// <inheritdoc/>
        public BlendshapeResult Solve(IReadOnlyDictionary<MediaPipeBlendshapes, float> weights)
        {
            if (weights == null || weights.Count <= 0)
                return null;

            var (blendshapes, leftEye, rightEye) = HolisticSolver.SolvePerfectly(weights);
            return new BlendshapeResult
            {
                PerfectSyncBlendshapes = blendshapes,
                LeftEye = leftEye,
                RightEye = rightEye,
            };
        }
    }
}
