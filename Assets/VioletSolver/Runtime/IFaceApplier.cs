using System.Collections.Generic;
using VRM;
using MediaPipeBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver
{
    public interface IFaceApplier
    {
        void Apply(IReadOnlyDictionary<BlendShapePreset, float> blendshapes);
        void Apply(IReadOnlyDictionary<MediaPipeBlendshapes, float> blendshapes);
    }
}
