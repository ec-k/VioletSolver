using System;
using System.Collections.Generic;
using UnityEngine;
using VioletSolver.Network;
using VioletSolver.Landmarks;
using Cysharp.Threading.Tasks;

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

        internal LandmarkHandler()
        {
            var faceLandmarkLength = 478; 
            var poseLength = (int)HolisticPose.PoseLandmarks.Types.LandmarkIndex.PoseLandmarkLength;
            var handLength = (int)HolisticPose.HandLandmarks.Types.LandmarkIndex.HandLandmarkLength;
            _landmarks = new HolisticLandmarks(faceLandmarkLength);
            _poseLandmarkFilters = new();
            _leftHandLandmarkFilters = new();
            _rightHandLandmarkFilters = new();
            _faceLandmarkFilters = new();
            MpBlendshapes = new();

            var cutConfidence = 0.3f;
            var smoothingFactor = 0.5f;
            var handSmoothingFactor = 0.1f;
            var minCutoff = 1f;
            var slope = 0.3f;
            var dCutoff = 1f;

            _poseLandmarkFilters.Add(new ConfidenceFilter(0.3f, 0.05f, cutConfidence));
            _poseLandmarkFilters.Add(new OneEuroFilter(poseLength, minCutoff, slope, dCutoff));

            _leftHandLandmarkFilters.Add(new ConfidenceFilter(0.3f, 0.05f, cutConfidence));
            _leftHandLandmarkFilters.Add(new OneEuroFilter(handLength, minCutoff, slope, dCutoff));

            _rightHandLandmarkFilters.Add(new ConfidenceFilter(0.3f, 0.05f, cutConfidence));
            _rightHandLandmarkFilters.Add(new OneEuroFilter(handLength, minCutoff, slope, dCutoff));


            //_faceLandmarkFilters.Add(new SingleExponentialSmoothingFilter(faceLandmarkLength, smoothingFactor));
            _faceLandmarkFilters.Add(new OneEuroFilter(faceLandmarkLength, minCutoff, slope, dCutoff));

        }
        internal LandmarkHandler(IHolisticLandmarks landmarks)
        {
            _landmarks = landmarks;
        }

        /// <summary>
        ///     Update landmarks: gets landmarks, filters them and assign to this.Landmarks.
        /// </summary>
        /// 
        async internal void Update() 
        {
            // Update landmarks.
            var results = _landmarkReceiveServer.Results;
            if (results == null || 
                results.PoseLandmarks == null) 
                return;
            _landmarks.UpdateLandmarks(results, _landmarkReceiveServer.Time);
            var resultedLandmarks = _landmarks;

            //// Apply filters
            {
                await UniTask.WhenAll(
                    UniTask.RunOnThreadPool(() => ApplyFilters(_poseLandmarkFilters, resultedLandmarks.Pose)),
                    UniTask.RunOnThreadPool(() => ApplyFilters(_rightHandLandmarkFilters, resultedLandmarks.RightHand)),
                    UniTask.RunOnThreadPool(() => ApplyFilters(_leftHandLandmarkFilters, resultedLandmarks.LeftHand)),
                    UniTask.RunOnThreadPool(() => ApplyFilters(_faceLandmarkFilters, resultedLandmarks.Face)));
            }

            _landmarks = resultedLandmarks;
        }

        void ApplyFilters(List<ILandmarkFilter> filters, ILandmarks landmarks)
        {
            if (filters != null)
                foreach (var filter in filters)
                    landmarks = filter.Filter(landmarks);
        }

        internal void UpdateBlendshapes()
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
