using System;
using System.Collections.Generic;
using UnityEngine;
using VRM;
using mpBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver.Pose
{
    // This class does
    //  1. get landmarks from udp by LandmarkReceiver.
    //  2. filters pose
    [Serializable]
    public class PoseHandler
    {
        AvatarPoseData _poseData;
        public AvatarPoseData PoseData => _poseData;

        public IReadOnlyDictionary<BlendShapePreset, float> BlendshapeWeights;
        public IReadOnlyDictionary<mpBlendshapes, float> PerfectSyncWeights;

        List<IAvatarPoseFilter> _vrmPoseFilters;
        List<IBlendshapeFilter> _blendshapeFilters;
        List<IPerfectSyncFilter> _perfectSyncFilters;

        public PoseHandler(
            IEnumerable<IAvatarPoseFilter> poseFilters = null,
            IEnumerable<IBlendshapeFilter> blendshapeFilters = null,
            IEnumerable<IPerfectSyncFilter> perfectSyncFilters = null)
        {
            _poseData          = new();
            BlendshapeWeights  = new Dictionary<BlendShapePreset, float>();
            PerfectSyncWeights = new Dictionary<mpBlendshapes, float>();

            _vrmPoseFilters     = poseFilters        is not null ? new(poseFilters)        : null;
            _blendshapeFilters  = blendshapeFilters  is not null ? new(blendshapeFilters)  : null;
            _perfectSyncFilters = perfectSyncFilters is not null ? new(perfectSyncFilters) : null;
        }

        // NOTE: The arguments are copied once to the variable "result" so that the data are stored as they are when there is no filter.
        public void Update(AvatarPoseData pose)
        {
            var result = pose;
            if (_vrmPoseFilters != null)
            {
                foreach (var filter in _vrmPoseFilters)
                {
                    result = filter.Filter(pose);
                }
            }
            _poseData = result;
        }

        public void Update(HumanBodyBones boneName, Quaternion value)
        {
            _poseData[boneName] = value;
        }


        // NOTE: The arguments are copied once to the variable "result" so that the data are stored as they are when there is no filter.
        public void Update(Dictionary<BlendShapePreset, float> weights)
        {
            var result = weights;
            if (_blendshapeFilters != null)
            {
                foreach (var filter in _blendshapeFilters)
                {
                    result = filter.Filter(weights);
                }
            }
            BlendshapeWeights = result;
        }
        public void Update(IReadOnlyDictionary<mpBlendshapes, float> weights)
        {
            var result = weights;
            if (_perfectSyncFilters != null)
                foreach (var filter in _perfectSyncFilters)
                {
                    result = filter.Filter(result);
                }
            PerfectSyncWeights = result;
        }
    }
}
