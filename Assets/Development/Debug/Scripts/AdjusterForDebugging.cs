using HumanoidPoseConnector;
using UnityEngine;

namespace VioletSolver.Debug {
    public class AdjusterForDebugging: MonoBehaviour
    {
        [SerializeField] HumanoidPoseSender _sender;
        void Start()
        {
            _sender.IsAvailable = true;
        }
    }
}
