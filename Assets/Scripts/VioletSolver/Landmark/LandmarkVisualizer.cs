using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace VioletSolver.Landmarks
{
    public class LandmarkVisualizer : MonoBehaviour
    {
        [SerializeField] GameObject src;
        LandmarkHandler _landmarkHandler;
        List<GameObject> _targets;

        void Start()
        {
            var initialCapacity = 30;
            _targets = new GameObject[initialCapacity].ToList();
            for (var i = 0; i < initialCapacity; i++)
            {
                SetTarget(PrimitiveType.Sphere, i, ref _targets);
            }

            _landmarkHandler = src.GetComponent<AvatarAnimator>().Landmarks;
        }

        void Update()
        {
            var lm = _landmarkHandler.Landmarks.Pose;
            if (lm == null)
                return;
            ExpandVisualsList(lm);
            for (var i = 0; i < lm.Count; i++)
            {
                _targets[i].transform.position = lm.Landmarks[i].Position;
            }
        }

        void ExpandVisualsList(ILandmarks lm)
        {
            var landmarkCount = lm.Count;
            var targetCount = _targets.Count;

            if (landmarkCount < targetCount)
            {
                for (var i = landmarkCount; i < targetCount; i++)
                {
                    Destroy(_targets[i].gameObject);
                }
                _targets.RemoveRange(landmarkCount, targetCount - landmarkCount);
            }
            else
            {
                CollectionUtils<GameObject>.ExpandList(lm.Count, ref _targets);
                for (var i = targetCount; i < landmarkCount; i++)
                {
                    SetTarget(PrimitiveType.Sphere, i, ref _targets);
                }
            }
        }

        void SetTarget(PrimitiveType shapeType, int index, ref List<GameObject> targets)
        {
            var obj = GameObject.CreatePrimitive(shapeType);
            obj.transform.localScale = Vector3.one * 0.1f;
            obj.transform.SetParent(gameObject.transform);
            obj.name = index.ToString();

            targets[index] = obj;
        }
    }
}
