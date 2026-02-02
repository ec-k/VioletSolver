using System.Collections.Generic;
using VioletSolver.Solver;
using MediaPipeBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver
{
    public class PerfectSyncBlendshapeSolver : IBlendshapeSolver
    {
        public BlendshapeResult Solve(Dictionary<MediaPipeBlendshapes, float> weights)
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
