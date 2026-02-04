using System.Collections.Generic;
using VRM;
using mpBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

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

    internal interface IPerfectSyncFilter
    {
        public IReadOnlyDictionary<mpBlendshapes, float> Filter(IReadOnlyDictionary<mpBlendshapes, float> blendshapes);
    }
}
