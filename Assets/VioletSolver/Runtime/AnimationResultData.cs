using System.Collections.Generic;
using System.Linq;
using VioletSolver.Pose;
using VRM;
using mpBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver
{
    public class AnimationResultData
    {
        public AvatarPoseData PoseData;
        public Dictionary<BlendShapePreset, float> VrmBlendshapes;
        public IReadOnlyDictionary<mpBlendshapes, float> PerfectSyncBlendshapes;

        public AnimationResultData Copy() => new AnimationResultData
        {
            PoseData = PoseData?.Copy(),
            VrmBlendshapes = VrmBlendshapes is not null ? new Dictionary<BlendShapePreset, float>(VrmBlendshapes) : null,
            PerfectSyncBlendshapes = PerfectSyncBlendshapes is not null ? PerfectSyncBlendshapes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) : null,
        };
    }
}
