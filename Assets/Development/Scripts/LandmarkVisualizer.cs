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

        [Header("Graph Offsets")]
        [SerializeField, Tooltip("Default: (0, 0, 1)")]      Vector3 _posePosOffset = new(0, 0, 1f);
        [SerializeField, Tooltip("Default: (-0.5, 0, 0.5)")] Vector3 _lHandPosOffset = new(-0.5f, 0, 0.5f);
        [SerializeField, Tooltip("Default: (0.5, 0, 0.5)")]  Vector3 _rHandPosOffset = new(0.5f, 0, 0.5f);

        [Header("Graph Size")]
        [SerializeField, Tooltip("Default: 0.05")] float _poseTargetSize = 0.05f;
        [SerializeField, Tooltip("Default: 0.01")] float _handTargetSize = 0.01f;

        GraphVisualizer _poseLandmarkVisualizer;
        GraphVisualizer _leftHandLandmarkVisualizer;
        GraphVisualizer _rightHandLandmarkVisualizer;

        [Header("Graph Materials")]
        [SerializeField] Material _poseVisualMaterial;
        [SerializeField] Material _handVisualMaterial;

        [Header("External Control")]
        [SerializeField] bool _isExternallyControlled = false;
        float _scale = 1f;

        void Reset()
        {
            _posePosOffset  = new(0, 0, 1f);
            _lHandPosOffset = new(-0.5f, 0, 0.5f);
            _rHandPosOffset = new(0.5f, 0, 0.5f);
            _poseTargetSize = 0.05f;
            _handTargetSize = 0.01f;
        }

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
        }

        void Update()
        {
            if (_isExternallyControlled)
                return;

            UpdateVisualization();
        }

        /// <summary>
        /// Sets the scale factor to apply to pose landmarks.
        /// Call this before UpdateVisualization() when externally controlled.
        /// </summary>
        public void SetScale(float scale)
        {
            _scale = scale;
        }

        /// <summary>
        /// Updates the landmark visualization.
        /// Call this externally when _isExternallyControlled is true.
        /// </summary>
        public void UpdateVisualization()
        {
            // Visualize Pose
            {
                var lm = _landmarkHandler.Landmarks.Pose;
                if (lm != null)
                {
                    _poseLandmarkVisualizer.UpdateGraphVisual(ExtractPositions(lm, _scale), _posePosOffset);
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

        Vector3[] ExtractPositions(in ILandmarkList lm, float scale = 1f)
        {
            var pos = new Vector3[lm.Count];
            for(var i = 0;i < lm.Count;i++)
                pos[i] = lm.Landmarks[i].Position * scale;
            return pos;
        }
    }
}
