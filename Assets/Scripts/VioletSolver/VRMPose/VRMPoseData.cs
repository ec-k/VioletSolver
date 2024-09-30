using UnityEngine;

namespace VioletSolver
{
    public struct IVRMPose
    {
        public Quaternion[] Bones { get; set; }
        public int Count => Bones.Length;
    }
}
