using System.Collections.Generic;
using UnityEngine;
using VioletSolver.Pose;
using VRM;
using mpBlendshapes = HolisticPose.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver
{
    public struct AnimationResultData
    {
        public AvatarPoseData PoseData;
        public Dictionary<BlendShapePreset, float> VrmBlendshapes;
        public Dictionary<mpBlendshapes, float> PerfectSyncBlendshapes;
    }
}
