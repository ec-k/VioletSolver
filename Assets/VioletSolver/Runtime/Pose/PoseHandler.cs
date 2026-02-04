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
    //  3. apply pose to avatar
    [Serializable]
    internal class PoseHandler
    {
        AvatarPoseData _poseData;
        internal AvatarPoseData PoseData => _poseData;

        internal Dictionary<BlendShapePreset, float> BlendshapeWeights;
        internal IReadOnlyDictionary<mpBlendshapes, float> PerfectSyncWeights;
        [SerializeField] List<IAvatarPoseFilter> _vrmPoseFilters;
        List<IBlendshapeFilter> _blendshapeFilters;
        List<IPerfectSyncFilter> _perfectSyncFilters;

        internal PoseHandler(
            IEnumerable<IAvatarPoseFilter> poseFilters = null,
            IEnumerable<IBlendshapeFilter> blendshapeFilters = null,
            IEnumerable<IPerfectSyncFilter> perfectSyncFilters = null)
        {
            _poseData = new();
            BlendshapeWeights = new();
            PerfectSyncWeights = new Dictionary<mpBlendshapes, float>();

            _vrmPoseFilters = poseFilters is not null ? new(poseFilters) : new();
            _blendshapeFilters = blendshapeFilters is not null ? new(blendshapeFilters) : new();
            _perfectSyncFilters = perfectSyncFilters is not null ? new(perfectSyncFilters) : new();
        }

        // NOTE: The arguments are copied once to the variable "result" so that the data are stored as they are when there is no filter.
        internal void Update(AvatarPoseData pose)
        {
            var result = pose;
            if(_vrmPoseFilters != null) 
                foreach (var filter in _vrmPoseFilters)
                {
                    result = filter.Filter(pose);
                }
            _poseData = result;
        }

        internal void Update(HumanBodyBones boneName, Quaternion value)
        {
            _poseData[boneName] = value;
        }


        // NOTE: The arguments are copied once to the variable "result" so that the data are stored as they are when there is no filter.
        internal void Update(Dictionary<BlendShapePreset, float> weights)
        {
            var result = weights;
            if (_blendshapeFilters != null)
                foreach (var filter in _blendshapeFilters)
                {
                    result = filter.Filter(weights);
                }
            BlendshapeWeights = result;
        }
        internal void Update(IReadOnlyDictionary<mpBlendshapes, float> weights)
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
