using UnityEngine;
using System.Collections.Generic;
using VioletSolver.Landmarks;

namespace VioletSolver.Debug
{
    public class LandmarkVisualizer : MonoBehaviour
    {
        [SerializeField] AvatarAnimator _avatarAnimator;
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

        void Start()
        {
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

            _landmarkHandler = _avatarAnimator.Landmarks;
        }

        void Update()
        {
            // Visualize Pose
            {
                var lm = _landmarkHandler.Landmarks.Pose;
                if (lm != null)
                {
                    _poseLandmarkVisualizer.UpdateGraphVisual(ExtractPositions(lm), _posePosOffset);
                    //ExpandVisualsList(lm, _poseTargetSize);
                    //for (var i = 0; i < lm.Count; i++)
                    //{
                    //    _poseTargets[i].transform.position = lm.Landmarks[i].Position;
                    //}
                }
            }
            // Visualize Left Hand
            {
                var lm = _landmarkHandler.Landmarks.LeftHand;
                if (lm != null)
                {
                    _leftHandLandmarkVisualizer.UpdateGraphVisual(ExtractPositions(lm), _lHandPosOffset);

                }
                //    return;
                //ExpandVisualsList(lm, _handTargetSize);
                //for (var i = 0; i < lm.Count; i++)
                //{
                //    _lHandTargets[i].transform.position = lm.Landmarks[i].Position + _lHandPosOffset;
                //}
            }
            // Visualize Right Hand
            {
                var lm = _landmarkHandler.Landmarks.RightHand;
                if (lm != null)
                {
                    _rightHandLandmarkVisualizer.UpdateGraphVisual(ExtractPositions(lm), _rHandPosOffset);
                }
                //    return;
                //ExpandVisualsList(lm, _handTargetSize);
                //for (var i = 0; i < lm.Count; i++)
                //{
                //    _rHandTargets[i].transform.position = lm.Landmarks[i].Position + _rHandPosOffset;
                //}
            }
        }

        Vector3[] ExtractPositions(ILandmarks lm)
        {
            var pos = new Vector3[lm.Count];
            for(var i = 0;i < lm.Count;i++) 
                pos[i] = lm.Landmarks[i].Position;
            return pos;
        }

        //void ExpandVisualsList(ILandmarks lm, float targetSize)
        //{
        //    var landmarkCount = lm.Count;
        //    var targetCount = _poseTargets.Count;

        //    if (landmarkCount < targetCount)
        //    {
        //        for (var i = landmarkCount; i < targetCount; i++)
        //        {
        //            Destroy(_poseTargets[i].gameObject);
        //        }
        //        _poseTargets.RemoveRange(landmarkCount, targetCount - landmarkCount);
        //    }
        //    else
        //    {
        //        CollectionUtils<GameObject>.ExpandList(lm.Count, ref _poseTargets);
        //        for (var i = targetCount; i < landmarkCount; i++)
        //        {
        //            SetTarget(PrimitiveType.Sphere, i, targetSize, ref _poseTargets);
        //        }
        //    }
        //}

        //void SetTarget(PrimitiveType shapeType, int index, float targetSize, ref List<GameObject> targets)
        //{
        //    var obj = GameObject.CreatePrimitive(shapeType);
        //    obj.transform.localScale = Vector3.one * targetSize;
        //    obj.transform.SetParent(gameObject.transform);
        //    obj.name = index.ToString();

        //    targets[index] = obj;
        //}
    }
}
