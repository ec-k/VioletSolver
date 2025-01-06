using System;
using System.Collections.Generic;
using UnityEngine;
using VioletSolver.Network;
using VioletSolver.Landmarks;

using MediaPipeBlendshapes = HolisticPose.Blendshapes.Types.BlendshapesIndex;

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
        public Dictionary<MediaPipeBlendshapes, float> MpBlendshapes;
        List<ILandmarkFilter> _poseLandmarkFilters;
        List<ILandmarkFilter> _leftHandLandmarkFilters;
        List<ILandmarkFilter> _rightHandLandmarkFilters;
        List<ILandmarkFilter> _faceLandmarkFilters;
        [SerializeField] LandmarkReceiver _landmarkReceiveServer;

        // for debugging
        GameObject[] _balls;

        public LandmarkHandler()
        {
            _landmarks = new HolisticLandmarks(30); // "30" is temporal number. There are no meanings and intentions.
            _poseLandmarkFilters = new();
            _leftHandLandmarkFilters = new();
            _rightHandLandmarkFilters = new();
            _faceLandmarkFilters = new();
            MpBlendshapes = new();

            var cutConfidence = 0f;
            var smoothingFactor = 0.5f;

            _poseLandmarkFilters.Add(new TransformCoordination());
            _poseLandmarkFilters.Add(new ConfidenceFilter(0.3f, 0.05f, cutConfidence));
            _poseLandmarkFilters.Add(new LowPassFilter(smoothingFactor));

            _leftHandLandmarkFilters.Add(new ConfidenceFilter(0.3f, 0.05f, cutConfidence));
            _leftHandLandmarkFilters.Add(new LowPassFilter(smoothingFactor));

            _rightHandLandmarkFilters.Add(new ConfidenceFilter(0.3f, 0.05f, cutConfidence));
            _rightHandLandmarkFilters.Add(new LowPassFilter(smoothingFactor));

            _faceLandmarkFilters.Add(new LowPassFilter(smoothingFactor));

        }
        public LandmarkHandler(IHolisticLandmarks landmarks)
        {
            _landmarks = landmarks;
        }

        /// <summary>
        ///     Update landmarks: gets landmarks, filters them and assign to this.Landmarks.
        /// </summary>
        /// 
        public void Update() 
        {
            // Update landmarks.
            var results = _landmarkReceiveServer.Results;
            if (results == null || 
                results.poseLandmarks == null) 
                return;
            _landmarks.UpdateLandmarks(results, _landmarkReceiveServer.Time);
            var resultedLandmarks = _landmarks;

            // Apply filters
            {
                if (_poseLandmarkFilters != null)
                    foreach (var filter in _poseLandmarkFilters)
                        resultedLandmarks.Pose = filter.Filter(resultedLandmarks.Pose);
                if (_leftHandLandmarkFilters != null)
                    foreach (var filter in _leftHandLandmarkFilters)
                        resultedLandmarks.LeftHand = filter.Filter(resultedLandmarks.LeftHand);
                if (_rightHandLandmarkFilters != null)
                    foreach (var filter in _rightHandLandmarkFilters)
                        resultedLandmarks.RightHand = filter.Filter(resultedLandmarks.RightHand);
                if (_faceLandmarkFilters != null)
                    foreach (var filter in _faceLandmarkFilters)
                        resultedLandmarks.Face = filter.Filter(resultedLandmarks.Face);
            }

            _landmarks = resultedLandmarks;
        }

        public void UpdateBlendshapes()
        {
            // Update blendshapes
            var results = _landmarkReceiveServer.Results;
            if (results.FaceResults == null ||
                results.FaceResults.Blendshapes == null ||
                results.FaceResults.Blendshapes.Scores == null)
                return;
            {
                var tmpArray = Enum.GetValues(typeof(MediaPipeBlendshapes));
                foreach (var value in tmpArray)
                {
                    var mpBlendshapeIndex = (MediaPipeBlendshapes)value;
                    try
                    {
                        MpBlendshapes[mpBlendshapeIndex] = results.FaceResults.Blendshapes.Scores[(int)mpBlendshapeIndex];
                    }
                    catch { }
                }
            }

        }
    }
}
