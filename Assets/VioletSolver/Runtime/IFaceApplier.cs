using System.Collections.Generic;
using VRM;
using MediaPipeBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver
{
    public interface IFaceApplier
    {
        void Apply(Dictionary<BlendShapePreset, float> blendshapes);
        void Apply(Dictionary<MediaPipeBlendshapes, float> blendshapes);
    }
}
