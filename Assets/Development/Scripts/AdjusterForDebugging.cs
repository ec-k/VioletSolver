using HumanoidPoseConnector;
using UnityEngine;

namespace VioletSolver.Development {
    public class AdjusterForDebugging: MonoBehaviour
    {
        [SerializeField] HumanoidPoseSender _sender;
        void Start()
        {
            _sender.IsAvailable = true;
        }
    }
}
