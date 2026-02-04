using System.Collections.Generic;
using VioletSolver.Pose;
using VRM;
using mpBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver
{
    public class AnimationResultData
    {
        public AvatarPoseData PoseData;
        public IReadOnlyDictionary<BlendShapePreset, float> VrmBlendshapes;
        public IReadOnlyDictionary<mpBlendshapes, float> PerfectSyncBlendshapes;

        /// <summary>
        /// Creates a deep copy of this instance.
        /// </summary>
        public AnimationResultData Copy() => new AnimationResultData
        {
            PoseData = PoseData?.Copy(),
            VrmBlendshapes = VrmBlendshapes is not null ? new Dictionary<BlendShapePreset, float>(VrmBlendshapes) : null,
            PerfectSyncBlendshapes = PerfectSyncBlendshapes is not null ? new Dictionary<mpBlendshapes, float>(PerfectSyncBlendshapes) : null,
        };
    }
}
