using System;
using System.Collections.Generic;
using VioletSolver.Network;
using VioletSolver.Landmarks;
using Cysharp.Threading.Tasks;

using MediaPipeBlendshapes = HolisticPose.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver
{
    // This class does
    //  1. gets landmarks
    //  2. filters landmarks
    public class LandmarkHandler
    {
        public IHolisticLandmarks Landmarks { get; private set; }
        public Dictionary<MediaPipeBlendshapes, float> MpBlendshapes;
        List<ILandmarkFilter> _poseLandmarkFilters;
        List<ILandmarkFilter> _leftHandLandmarkFilters;
        List<ILandmarkFilter> _rightHandLandmarkFilters;
        List<ILandmarkFilter> _faceLandmarkFilters;

        public LandmarkHandler(LandmarkReceiver receiver)
        {
            var faceLandmarkLength = 478; 
            Landmarks = new HolisticLandmarks(faceLandmarkLength);
            _poseLandmarkFilters = new();
            _leftHandLandmarkFilters = new();
            _rightHandLandmarkFilters = new();
            _faceLandmarkFilters = new();
            MpBlendshapes = new();

            receiver.OnLandmarksReceived += (results, receivedTime) => OnLandmarkReceived(results, receivedTime).Forget();
        }

        /// <summary>
        ///     Update landmarks: gets landmarks, filters them and assign to this.Landmarks.
        /// </summary>
        internal async UniTask OnLandmarkReceived(HolisticPose.HolisticLandmarks results, float receivedTime)
        {
            // Update landmarks.
            if (results == null ||
                results.PoseLandmarks == null)
                return;
            Landmarks.UpdateLandmarks(results, receivedTime);
            var resultedLandmarks = Landmarks;

            //// Apply filters
            {
                await UniTask.WhenAll(
                    UniTask.RunOnThreadPool(() => ApplyFilters(_poseLandmarkFilters, resultedLandmarks.Pose)),
                    UniTask.RunOnThreadPool(() => ApplyFilters(_rightHandLandmarkFilters, resultedLandmarks.RightHand)),
                    UniTask.RunOnThreadPool(() => ApplyFilters(_leftHandLandmarkFilters, resultedLandmarks.LeftHand)),
                    UniTask.RunOnThreadPool(() => ApplyFilters(_faceLandmarkFilters, resultedLandmarks.Face)));
            }

            Landmarks = resultedLandmarks;
            UpdateBlendshapes(results);
        }

        void ApplyFilters(List<ILandmarkFilter> filters, ILandmarks landmarks)
        {
            if (filters != null)
                foreach (var filter in filters)
                    landmarks = filter.Filter(landmarks);
        }

        internal void UpdateBlendshapes(HolisticPose.HolisticLandmarks results)
        {
            // Update blendshapes
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
