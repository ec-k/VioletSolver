using System;
using UnityEngine;

namespace VioletSolver.Pose
{
    internal static class PoseDataUtils
    {
        internal static bool Equals(AvatarPoseData pose1, AvatarPoseData pose2)
        {
            var hbbs = Enum.GetValues(typeof(HumanBodyBones));
            foreach (var hbb in hbbs)
            {
                var boneName = (HumanBodyBones) hbb;
                if (pose1[boneName] != pose2[boneName])
                    return false;
            }
            return true;
        }
    }
}
