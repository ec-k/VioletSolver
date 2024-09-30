using System;

namespace VioletSolver
{
    public class ConfidenceFilter: ILandmarkFilter
    {
        public ILandmarks Filter(ILandmarks landmarks, float amount)
        {
            for (var i = 0; i < landmarks.Count; i++)
            {
                FilterOnOneLandmark(landmarks.Landmarks[i], amount);
            }
            return landmarks;
        }
        static Landmark FilterOnOneLandmark(Landmark landmark, float amount)
        {
            var _amount = Math.Clamp(amount, 0, 1);
            var result = new Landmark(
                landmark.X * _amount * landmark.Confidence,
                landmark.Y * _amount * landmark.Confidence,
                landmark.Z * _amount * landmark.Confidence);
            return result;
        }
    }
}
