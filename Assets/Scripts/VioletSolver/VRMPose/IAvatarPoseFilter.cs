using System.Collections.Generic;
using VRM;

namespace VioletSolver
{
    public interface IAvatarPoseFilter
    {
        public AvatarPoseData Filter(AvatarPoseData boneRotations, float amount);
    }

    public interface IBlendshapeFilter
    {
        public Dictionary<BlendShapePreset, float> Filter(Dictionary<BlendShapePreset, float> blendshapes, float amount);
    }
}
