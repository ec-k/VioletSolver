using System;
using UnityEngine;

namespace VioletSolver.Pose
{
    internal class Interpolator : IAvatarPoseFilter
    {
        AvatarPoseData _current;

        internal Interpolator()
        {
            _current = new AvatarPoseData();
        }
        public AvatarPoseData Filter(AvatarPoseData data)
        {
            var result = new AvatarPoseData();

            var deltaDataTime = data.time - _current.time;
            var lerpAmount = Mathf.Clamp01(Time.deltaTime / deltaDataTime);

            var hbbs = Enum.GetValues(typeof(HumanBodyBones));
            foreach (var hbb in hbbs)
            {
                var boneName = (HumanBodyBones) hbb;
                result[boneName] = Quaternion.Slerp(_current[boneName], data[boneName], lerpAmount);
            }

            result.time = Mathf.Lerp(_current.time, data.time, lerpAmount);

            //Debug.Log(deltaDataTime);

            //Debug.Log(PoseDataUtils.Equals(data, result));
            //if (!PoseDataUtils.Equals(data, _current))
                _current = result;
            return result;
        }
    }
}
