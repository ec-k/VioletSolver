using System.Collections.Generic;
using VioletSolver.Pose;
using VRM;
using mpBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver
{
    public struct AnimationResultData
    {
        public AvatarPoseData PoseData;
        public Dictionary<BlendShapePreset, float> VrmBlendshapes;
        public Dictionary<mpBlendshapes, float> PerfectSyncBlendshapes;
    }
}
