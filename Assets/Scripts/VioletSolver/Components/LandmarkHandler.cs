using System;
using System.Collections.Generic;
using UnityEngine;
using VioletSolver.Network;

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
        [SerializeField] List<ILandmarkFilter> _landmarkFilters;
        [SerializeField] LandmarkReceiver _landmarkReceiveServer;

        // for debugging
        GameObject[] _balls;

        public LandmarkHandler()
        {
            _landmarks = new HolisticLandmarks(30); // "30" is temporal number. There are no meanings and intentions.
            _landmarkFilters = new();
            MpBlendshapes = new();

            _landmarkFilters.Add(new TransformCoordination());
            _landmarkFilters.Add(new ConfidenceFilter(0.3f, 0.05f));
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
            if (_landmarkFilters != null)
                foreach (var filter in _landmarkFilters)
                {
                    resultedLandmarks.Pose = filter.Filter(resultedLandmarks.Pose);
                    //resultedLandmarks.LeftHand = filter.Filter(resultedLandmarks.LeftHand, _filterAmount);
                    //resultedLandmarks.RightHand = filter.Filter(resultedLandmarks.RightHand, _filterAmount);
                    //resultedLandmarks.Face = filter.Filter(resultedLandmarks.Face, _filterAmount);
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
