using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using VRM;
using MediaPipeBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver.Solver.Face
{
    public class BlendshapeResult
    {
        public Dictionary<BlendShapePreset, float> VrmBlendshapes;
        public IReadOnlyDictionary<MediaPipeBlendshapes, float> PerfectSyncBlendshapes;
        public Quaternion LeftEye;
        public Quaternion RightEye;
    }

    /// <summary>
    /// Interface for resolving avatar blendshapes from MediaPipe blendshape weights.
    /// </summary>
    public interface IBlendshapeSolver
    {
        /// <summary>
        /// Resolves avatar blendshapes and eye rotations from MediaPipe blendshape weights.
        /// </summary>
        /// <param name="weights">MediaPipe blendshape weights.</param>
        /// <returns>The resolved blendshape result, or null if weights are invalid.</returns>
        BlendshapeResult Solve(IReadOnlyDictionary<MediaPipeBlendshapes, float> weights);
    }
}
