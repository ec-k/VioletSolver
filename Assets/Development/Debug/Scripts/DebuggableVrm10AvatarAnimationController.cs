using UnityEngine;
using VioletSolver.Samples;

namespace VioletSolver.Debug
{
    public class DebuggableVrm10AvatarAnimationController : Vrm10AvatarAnimationController
    {
        [Header("Debug")]
        [SerializeField] LandmarkVisualizer _landmarkVisualizer;

        protected override void Awake()
        {
            base.Awake();

            if (_landmarkVisualizer is not null)
                _landmarkVisualizer.Initialize(_landmarkHandler);
            else
                UnityEngine.Debug.LogWarning("LandmarkVisualizer is not assigned. Landmark visualization will not work.", this);
        }

        protected override void OnPostUpdate(float scale)
        {
            if (_landmarkVisualizer is not null)
            {
                _landmarkVisualizer.SetScale(scale);
                _landmarkVisualizer.UpdateVisualization();
            }
        }
    }
}
