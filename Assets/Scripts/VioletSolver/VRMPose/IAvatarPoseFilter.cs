using System.Collections.Generic;
using VRM;

namespace VioletSolver
{
    public interface IAvatarPoseFilter
    {
        public AvatarPoseData Filter(AvatarPoseData boneRotations);
    }

    public interface IBlendshapeFilter
    {
        public Dictionary<BlendShapePreset, float> Filter(Dictionary<BlendShapePreset, float> blendshapes);
    }
}
