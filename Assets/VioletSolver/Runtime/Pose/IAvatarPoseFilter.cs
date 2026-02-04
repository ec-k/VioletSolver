using System.Collections.Generic;
using VRM;
using mpBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver.Pose
{
    public interface IAvatarPoseFilter
    {
        public AvatarPoseData Filter(AvatarPoseData boneRotations);
    }

    public interface IBlendshapeFilter
    {
        public Dictionary<BlendShapePreset, float> Filter(Dictionary<BlendShapePreset, float> blendshapes);
    }

    public interface IPerfectSyncFilter
    {
        public IReadOnlyDictionary<mpBlendshapes, float> Filter(IReadOnlyDictionary<mpBlendshapes, float> blendshapes);
    }
}
