using System.Collections.Generic;
using VRM;
using MediaPipeBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver.FaceApplier
{
    /// <summary>
    /// Interface for applying blendshapes to avatar facial expressions.
    /// </summary>
    public interface IFaceApplier
    {
        /// <summary>
        /// Applies standard VRM blendshapes to the avatar.
        /// </summary>
        /// <param name="blendshapes">Mapping of VRM BlendShapePreset to weight values.</param>
        void Apply(IReadOnlyDictionary<BlendShapePreset, float> blendshapes);

        /// <summary>
        /// Applies MediaPipe blendshapes (Perfect Sync) to the avatar.
        /// </summary>
        /// <param name="blendshapes">Mapping of MediaPipe blendshape indices to weight values.</param>
        void Apply(IReadOnlyDictionary<MediaPipeBlendshapes, float> blendshapes);
    }
}
