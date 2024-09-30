using UnityEngine;

namespace VioletSolver
{
    public interface IVRMPoseFilter
    {
        public Quaternion[] Filter(Quaternion[] boneRotations);
    }
}
