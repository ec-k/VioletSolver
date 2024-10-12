using System;
using UnityEngine;

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
        [SerializeField] IAvatarPoseFilter[] _vrmPoseFilters;
        [SerializeField] float _filterAmount = 1f;

        public AvatarPoseHandler()
        {
            _poseData = new AvatarPoseData();
        }

        public void Update(AvatarPoseData poseData)
        {
            var pose = poseData;
            if(_vrmPoseFilters != null) 
                foreach (var filter in _vrmPoseFilters)
                {
                    pose = filter.Filter(pose, _filterAmount);
                }
            _poseData = pose;
        }

    }
}
