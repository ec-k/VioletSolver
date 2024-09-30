using System;
using UnityEngine;
using VioletSolver.Network;

namespace VioletSolver
{
    // This class does
    //  1. gets landmarks
    //  2. filters landmarks
    [Serializable]
    public class LandmarkHandler
    {
        IHolisticLandmarks _landmarks;
        public IHolisticLandmarks Landmarks => _landmarks;
        [SerializeField] ILandmarkFilter[] _landmarkFilters;
        [SerializeField] float _filterAmount = 1f;
        [SerializeField] LandmarkReceiveServer _landmarkReceiveServer;
        public LandmarkHandler() { }

        public void Update() 
        {
            _landmarks.UpdateLandmarks(_landmarkReceiveServer.Results);
            var resultedLandmarks = _landmarks;
            foreach (var filter in _landmarkFilters)
            {
                resultedLandmarks.Pose = filter.Filter(resultedLandmarks.Pose, _filterAmount);
                resultedLandmarks.LeftHand = filter.Filter(resultedLandmarks.LeftHand, _filterAmount);
                resultedLandmarks.RightHand = filter.Filter(resultedLandmarks.RightHand, _filterAmount);
                resultedLandmarks.Face = filter.Filter(resultedLandmarks.Face, _filterAmount);
            }
            _landmarks = resultedLandmarks;
        }
    }
}
