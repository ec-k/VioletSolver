using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using VRM;
using MediaPipeBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver
{
    public class BlendshapeResult
    {
        public Dictionary<BlendShapePreset, float> VrmBlendshapes;
        public IReadOnlyDictionary<MediaPipeBlendshapes, float> PerfectSyncBlendshapes;
        public Quaternion LeftEye;
        public Quaternion RightEye;
    }

    public interface IBlendshapeSolver
    {
        BlendshapeResult Solve(IReadOnlyDictionary<MediaPipeBlendshapes, float> weights);
    }
}
