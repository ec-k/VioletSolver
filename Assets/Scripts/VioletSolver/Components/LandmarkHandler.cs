using System;
using System.Collections.Generic;
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
        ILandmarks _landmarks;
        public ILandmarks Landmarks => _landmarks;
        [SerializeField] List<ILandmarkFilter> _landmarkFilters;
        [SerializeField] float _filterAmount = 1f;
        [SerializeField] LandmarkReceiver _landmarkReceiveServer;

        // for debugging
        GameObject[] _balls;

        public LandmarkHandler()
        {
            _landmarks = new PoseLandmarks(30); // number 30 is temporal number. There are no meanings and intentions.
            _landmarkFilters = new List<ILandmarkFilter>();

            _landmarkFilters.Add(new TransformCoordination());
            _landmarkFilters.Add(new ConfidenceFilter());
        }
        public LandmarkHandler(ILandmarks landmarks)
        {
            _landmarks = landmarks;
        }

        /// <summary>
        ///     Update landmarks: gets landmarks, filters them and assign to this.Landmarks.
        /// </summary>
        /// 
        public void Update() 
        {
            if (_landmarkReceiveServer.Results == null || 
                _landmarkReceiveServer.Results.PoseLandmarks == null) 
                return;
            _landmarks.UpdateLandmarks(_landmarkReceiveServer.Results.PoseLandmarks);
            var resultedLandmarks = _landmarks;
            if (_landmarkFilters != null)
                foreach (var filter in _landmarkFilters)
                {
                    resultedLandmarks = filter.Filter(resultedLandmarks, _filterAmount);
                    //resultedLandmarks.Pose = filter.Filter(resultedLandmarks.Pose, _filterAmount);
                    //resultedLandmarks.LeftHand = filter.Filter(resultedLandmarks.LeftHand, _filterAmount);
                    //resultedLandmarks.RightHand = filter.Filter(resultedLandmarks.RightHand, _filterAmount);
                    //resultedLandmarks.Face = filter.Filter(resultedLandmarks.Face, _filterAmount);
                }
            _landmarks = resultedLandmarks;
        }
    }
}
