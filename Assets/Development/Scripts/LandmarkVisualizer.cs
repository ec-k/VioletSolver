using UnityEngine;
using System.Collections.Generic;
using VioletSolver.Landmarks;

namespace VioletSolver.Development
{
    internal class LandmarkVisualizer : MonoBehaviour
    {
        LandmarkHandler _landmarkHandler;

        List<GameObject> _poseTargets;
        List<GameObject> _lHandTargets;
        List<GameObject> _rHandTargets;

        Vector3 _posePosOffset;
        Vector3 _lHandPosOffset;
        Vector3 _rHandPosOffset;

        float _poseTargetSize = 0.05f;
        float _handTargetSize = 0.01f;

        GraphVisualizer _poseLandmarkVisualizer;
        GraphVisualizer _leftHandLandmarkVisualizer;
        GraphVisualizer _rightHandLandmarkVisualizer;

        [SerializeField] Material _poseVisualMaterial;
        [SerializeField] Material _handVisualMaterial;

        // NOTE: Call this in Awake.
        public void Initialize(LandmarkHandler handler)
        {
            if(handler is null)
            {
                Debug.LogError("LandmarkHandler passed to LandmarkVisualizer.Initialize is null.", this);
                enabled = false;
                return;
            }
            _landmarkHandler = handler;
        }

        void Start()
        {
            if(_landmarkHandler is null)
            {
                Debug.LogError("LandmarkHandler is not assigned to LandmarkVisualizer.", this);
                enabled = false;
                return;
            }

            var poseNodeVisual = new NodeVisualOption(_poseTargetSize);
            var handNodeVisual = new NodeVisualOption(_handTargetSize);

            var poseGraph = LandmarkGraphGenerator.GenerateKinectPoseGraph();
            var handGraph = LandmarkGraphGenerator.GenerateHandGraph();

            _poseLandmarkVisualizer = new(poseGraph, poseNodeVisual, _poseVisualMaterial, "Pose Landmark");
            _leftHandLandmarkVisualizer = new(handGraph, handNodeVisual, _handVisualMaterial, "Left Hand Landmark");
            _rightHandLandmarkVisualizer = new(handGraph, handNodeVisual, _handVisualMaterial, "Right Hand Landmark");

            _poseLandmarkVisualizer.RootNode.transform.parent = gameObject.transform;
            _leftHandLandmarkVisualizer.RootNode.transform.parent = gameObject.transform;
            _rightHandLandmarkVisualizer.RootNode.transform.parent = gameObject.transform;

            _posePosOffset  = new(0     , 0, 1f);
            _lHandPosOffset = new(-0.5f , 0, 0.5f);
            _rHandPosOffset = new(0.5f  , 0, 0.5f);
        }

        void Update()
        {
            // Visualize Pose
            {
                var lm = _landmarkHandler.Landmarks.Pose;
                if (lm != null)
                {
                    _poseLandmarkVisualizer.UpdateGraphVisual(ExtractPositions(lm), _posePosOffset);
                }
            }
            // Visualize Left Hand
            {
                var lm = _landmarkHandler.Landmarks.LeftHand;
                if (lm != null)
                {
                    _leftHandLandmarkVisualizer.UpdateGraphVisual(ExtractPositions(lm), _lHandPosOffset);

                }
            }
            // Visualize Right Hand
            {
                var lm = _landmarkHandler.Landmarks.RightHand;
                if (lm != null)
                {
                    _rightHandLandmarkVisualizer.UpdateGraphVisual(ExtractPositions(lm), _rHandPosOffset);
                }
            }
        }

        Vector3[] ExtractPositions(ILandmarks lm)
        {
            var pos = new Vector3[lm.Count];
            for(var i = 0;i < lm.Count;i++) 
                pos[i] = lm.Landmarks[i].Position;
            return pos;
        }
    }
}
