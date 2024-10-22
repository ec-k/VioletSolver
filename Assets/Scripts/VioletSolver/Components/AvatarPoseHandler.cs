using Codice.CM.SEIDInfo;
using System;
using System.Collections.Generic;
using UnityEngine;
using VRM;

namespace VioletSolver
{
    // This class does
    //  1. get landmarks from udp by LandmarkReceiver.
    //  2. filters pose
    //  3. apply pose to avatar
    [Serializable]
    public class AvatarPoseHandler
    {
        AvatarPoseData _poseData;
        public AvatarPoseData PoseData => _poseData;

        public Dictionary<BlendShapePreset, float> BlendshapeWeights;
        [SerializeField] List<IAvatarPoseFilter> _vrmPoseFilters;
        List<IBlendshapeFilter> _blendshapeFilters;
        [SerializeField] float _filterAmount = 1f;


        public AvatarPoseHandler()
        {
            _poseData = new();
            BlendshapeWeights = new();

            _vrmPoseFilters = new();
            _vrmPoseFilters.Add(new LowPassFilter(0.1f));
            _vrmPoseFilters.Add(new Interpolator());
        }

        // NOTE: The arguments are copied once to the variable ÅgresultÅh so that the data are stored as they are when there is no filter.
        public void Update(AvatarPoseData pose)
        {
            var result = pose;
            if(_vrmPoseFilters != null) 
                foreach (var filter in _vrmPoseFilters)
                {
                    result = filter.Filter(pose, _filterAmount);
                }
            _poseData = result;
        }

        public void Update(HumanBodyBones boneName, Quaternion value)
        {
            _poseData[boneName] = value;
        }


        // NOTE: The arguments are copied once to the variable ÅgresultÅh so that the data are stored as they are when there is no filter.
        public void Update(Dictionary<BlendShapePreset, float> weights)
        {
            var result = weights;
            if (_blendshapeFilters != null)
                foreach (var filter in _blendshapeFilters)
                {
                    result = filter.Filter(weights, _filterAmount);
                }
            BlendshapeWeights = result;
        }
    }
}
