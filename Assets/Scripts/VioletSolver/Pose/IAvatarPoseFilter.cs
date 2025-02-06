using System.Collections.Generic;
using VRM;

namespace VioletSolver.Pose
{
    internal interface IAvatarPoseFilter
    {
        public AvatarPoseData Filter(AvatarPoseData boneRotations);
    }

    internal interface IBlendshapeFilter
    {
        public Dictionary<BlendShapePreset, float> Filter(Dictionary<BlendShapePreset, float> blendshapes);
    }
}
