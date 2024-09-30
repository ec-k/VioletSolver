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
        AvatarPoseData _avatarPoseData;
        [SerializeField] IVRMPoseFilter[] _vrmPoseFilters;

        public AvatarPoseHandler()
        {
            _avatarPoseData = new AvatarPoseData();
        }

    }
}
