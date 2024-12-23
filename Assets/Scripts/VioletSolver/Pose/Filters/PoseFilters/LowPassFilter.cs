using System;
using UnityEngine;

namespace VioletSolver.Pose
{
    public class LowPassFilter : IAvatarPoseFilter
    {
        AvatarPoseData _prev;
        float _amount;
        public LowPassFilter(float amount)
        {
            _prev = new AvatarPoseData();
            _amount = amount;
        }

        public AvatarPoseData Filter(AvatarPoseData pose)
        {
            var result = new AvatarPoseData();

            var hbbs = Enum.GetValues(typeof(HumanBodyBones));
            foreach(var hbb in hbbs)
            {
                var boneName = (HumanBodyBones) hbb;
                result[boneName] = Quaternion.Slerp(_prev[boneName], pose[boneName], _amount);
            }

            _prev = result;
            return result;
        }
    }
}
