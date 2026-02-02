using System.Collections.Generic;
using VioletSolver.Solver;
using MediaPipeBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver
{
    public class StandardBlendshapeSolver : IBlendshapeSolver
    {
        public BlendshapeResult Solve(Dictionary<MediaPipeBlendshapes, float> weights)
        {
            if (weights == null || weights.Count <= 0)
                return null;

            var (blendshapes, leftEye, rightEye) = HolisticSolver.Solve(weights);
            return new BlendshapeResult
            {
                VrmBlendshapes = blendshapes,
                LeftEye = leftEye,
                RightEye = rightEye,
            };
        }
    }
}
