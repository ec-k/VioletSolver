using System;
using System.Collections.Generic;
using VioletSolver.LandmarkProviders;
using VioletSolver.Landmarks;

using MediaPipeBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver
{
    // This class does
    //  1. gets landmarks
    //  2. filters landmarks
    public class LandmarkHandler
    {
        public HolisticLandmarks Landmarks { get; private set; }
        public Dictionary<MediaPipeBlendshapes, float> MpBlendshapes;

        public LandmarkHandler(ILandmarkProvider receiver)
        {
            var faceLandmarkLength = 478; 
            Landmarks = new HolisticLandmarks(faceLandmarkLength);
            MpBlendshapes = new();

            receiver.OnLandmarksReceived += (results, receivedTime) => OnLandmarkReceived(results, receivedTime);
        }

        /// <summary>
        ///     Update landmarks: gets landmarks, filters them and assign to this.Landmarks.
        /// </summary>
        internal void OnLandmarkReceived(HumanLandmarks.HolisticLandmarks results, float receivedTime)
        {
            // Update landmarks.
            if (results == null ||
                (results.MediaPipePoseLandmarks == null && results.KinectPoseLandmarks == null)
                )
                return;
            Landmarks.UpdateLandmarks(results, receivedTime);

            UpdateBlendshapes(results);
        }

        internal void UpdateBlendshapes(HumanLandmarks.HolisticLandmarks results)
        {
            // Update blendshapes
            if (results.FaceResults == null ||
                results.FaceResults.Blendshapes == null ||
                results.FaceResults.Blendshapes.Scores == null)
                return;

            var scores = results.FaceResults.Blendshapes.Scores;
                var tmpArray = Enum.GetValues(typeof(MediaPipeBlendshapes));
                foreach (var value in tmpArray)
                {
                    var mpBlendshapeIndex = (MediaPipeBlendshapes)value;

                if ((int)value < scores.Count)
                    MpBlendshapes[mpBlendshapeIndex] = scores[(int)value];
            }
        }
    }
}
