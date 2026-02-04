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
            Landmarks = new(faceLandmarkLength);
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
            if (results.FaceResults?.Blendshapes?.Scores is not { } scores)
                return;

            foreach (MediaPipeBlendshapes index in Enum.GetValues(typeof(MediaPipeBlendshapes)))
            {
                if ((int)index < scores.Count)
                    MpBlendshapes[index] = scores[(int)index];
            }
        }
    }
}
