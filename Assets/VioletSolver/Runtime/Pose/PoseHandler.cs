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
        internal Dictionary<mpBlendshapes, float> PerfectSyncWeights;
        [SerializeField] List<IAvatarPoseFilter> _vrmPoseFilters;
        List<IBlendshapeFilter> _blendshapeFilters;


        internal PoseHandler()
        {
            _poseData = new();
            BlendshapeWeights = new();
            PerfectSyncWeights = new();

            _vrmPoseFilters = new();
            _vrmPoseFilters.Add(new PoseUpSampler());
            //_vrmPoseFilters.Add(new LowPassFilter(0.6f));
            //_vrmPoseFilters.Add(new Interpolator());

            _blendshapeFilters = new();
            _blendshapeFilters.Add(new Blendshapes.LowPassFilter(0.6f));
            _blendshapeFilters.Add(new Blendshapes.EyeOpener(0.15f));   // completely opening is 0.0
            _blendshapeFilters.Add(new Blendshapes.EyeCloser(0.85f));   // completely closeing is 1.0
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
        internal void Update(Dictionary<mpBlendshapes, float> weights)
        {
            //var result = weights;
            //if (_blendshapeFilters != null)
            //    foreach (var filter in _blendshapeFilters)
            //    {
            //        result = filter.Filter(weights, _filterAmount);
            //    }
            //BlendshapeWeights = result;
            PerfectSyncWeights = weights;
        }
    }
}
