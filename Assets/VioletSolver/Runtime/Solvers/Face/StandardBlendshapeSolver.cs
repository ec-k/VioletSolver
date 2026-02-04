using System.Collections.Generic;
using VioletSolver.Solver;
using MediaPipeBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver.Solver.Face
{
    /// <summary>
    /// Solver that converts MediaPipe blendshapes to VRM BlendShapePresets using the standard method.
    /// </summary>
    public class StandardBlendshapeSolver : IBlendshapeSolver
    {
        /// <inheritdoc/>
        public BlendshapeResult Solve(IReadOnlyDictionary<MediaPipeBlendshapes, float> weights)
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
