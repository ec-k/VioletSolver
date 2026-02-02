using Codice.CM.Triggers;
using System;
using System.Collections.Generic;
using VioletSolver.LandmarkProviders;
using VioletSolver.Landmarks;

using MediaPipeBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver
{
    /// <summary>
    /// Receives and filters landmarks.
    /// </summary>
    public class LandmarkHandler
    {
        public HolisticLandmarks Landmarks { get; private set; }
        public Dictionary<MediaPipeBlendshapes, float> MpBlendshapes;

        public bool IsKinectPose { get; private set; }

        /// <summary>
        /// Initializes the LandmarkHandler and subscribes to the landmark received event.
        /// </summary>
        /// <param name="receiver">The provider that supplies landmarks.</param>
        public LandmarkHandler(ILandmarkProvider receiver)
        {
            var faceLandmarkLength = 478; 
            Landmarks = new HolisticLandmarks(faceLandmarkLength);
            MpBlendshapes = new();

            receiver.OnLandmarksReceived += (results, receivedTime) => OnLandmarkReceived(results, receivedTime);
        }

        /// <summary>
        /// Updates landmarks: gets landmarks and assigns them to this.Landmarks.
        /// </summary>
        internal void OnLandmarkReceived(HumanLandmarks.HolisticLandmarks results, float receivedTime)
        {
            // Update landmarks.
            if (results == null)
                return;
            Landmarks.UpdateLandmarks(results, receivedTime);

            IsKinectPose = results.KinectPoseLandmarks is not null;

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
