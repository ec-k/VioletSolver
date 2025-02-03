using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace VioletSolver.Landmarks
{
    public class LandmarkVisualizer : MonoBehaviour
    {
        [SerializeField] GameObject src;
        LandmarkHandler _landmarkHandler;
        List<GameObject> _poseTargets;
        List<GameObject> _lHandTargets;
        List<GameObject> _rHandTargets;

        Vector3 _lHandPosOffset;
        Vector3 _rHandPosOffset;

        float _poseTargetSize = 0.05f;
        float _handTargetSize = 0.01f;

        void Start()
        {
            _lHandPosOffset = new(-0.5f, 0, 0);
            _rHandPosOffset = new(0.5f, 0, 0);

            var initialCapacity = 30;
            _poseTargets = new GameObject[initialCapacity].ToList();
            _lHandTargets = new GameObject[initialCapacity].ToList();
            _rHandTargets = new GameObject[initialCapacity].ToList();
            for (var i = 0; i < initialCapacity; i++)
            {
                SetTarget(PrimitiveType.Sphere, i, _poseTargetSize, ref _poseTargets);
                SetTarget(PrimitiveType.Sphere, i, _handTargetSize, ref _lHandTargets);
                SetTarget(PrimitiveType.Sphere, i, _handTargetSize, ref _rHandTargets);
            }

            _landmarkHandler = src.GetComponent<AvatarAnimator>().Landmarks;
        }

        void Update()
        {
            // Visualize Pose
            {
                var lm = _landmarkHandler.Landmarks.Pose;
                if (lm == null)
                    return;
                ExpandVisualsList(lm, _poseTargetSize);
                for (var i = 0; i < lm.Count; i++)
                {
                    _poseTargets[i].transform.position = lm.Landmarks[i].Position;
                }
            }
            // Visualize Left Hand
            {
                var lm = _landmarkHandler.Landmarks.LeftHand;
                if (lm == null)
                    return;
                ExpandVisualsList(lm, _handTargetSize);
                for (var i = 0; i < lm.Count; i++)
                {
                    _lHandTargets[i].transform.position = lm.Landmarks[i].Position + _lHandPosOffset;
                }
            }
            // Visualize Right Hand
            {
                var lm = _landmarkHandler.Landmarks.RightHand;
                if (lm == null)
                    return;
                ExpandVisualsList(lm, _handTargetSize);
                for (var i = 0; i < lm.Count; i++)
                {
                    _rHandTargets[i].transform.position = lm.Landmarks[i].Position + _rHandPosOffset;
                }
            }
        }

        void ExpandVisualsList(ILandmarks lm, float targetSize)
        {
            var landmarkCount = lm.Count;
            var targetCount = _poseTargets.Count;

            if (landmarkCount < targetCount)
            {
                for (var i = landmarkCount; i < targetCount; i++)
                {
                    Destroy(_poseTargets[i].gameObject);
                }
                _poseTargets.RemoveRange(landmarkCount, targetCount - landmarkCount);
            }
            else
            {
                CollectionUtils<GameObject>.ExpandList(lm.Count, ref _poseTargets);
                for (var i = targetCount; i < landmarkCount; i++)
                {
                    SetTarget(PrimitiveType.Sphere, i, targetSize, ref _poseTargets);
                }
            }
        }

        void SetTarget(PrimitiveType shapeType, int index, float targetSize, ref List<GameObject> targets)
        {
            var obj = GameObject.CreatePrimitive(shapeType);
            obj.transform.localScale = Vector3.one * targetSize;
            obj.transform.SetParent(gameObject.transform);
            obj.name = index.ToString();

            targets[index] = obj;
        }
    }
}
