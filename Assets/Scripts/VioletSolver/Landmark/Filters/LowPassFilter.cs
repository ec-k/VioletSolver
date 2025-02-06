using System;
using System.Collections.Generic;

namespace VioletSolver.Landmarks
{
    internal class LowPassFilter : ILandmarkFilter
    {
        List<Landmark> _prevResults;
        float _smoothingFactor;

        internal LowPassFilter(float smootingFactor)
        {
            _smoothingFactor = smootingFactor;
            _prevResults = new List<Landmark>(30);
        }

        public ILandmarks Filter(ILandmarks landmarks)
        {
            for (var i = 0; i < Math.Min(landmarks.Count, _prevResults.Count); i++)
            {
                landmarks.Landmarks[i] = Filter(_prevResults[i], landmarks.Landmarks[i], _smoothingFactor);
            }

            UpdatePrevResults(landmarks);

            return landmarks;
        }

        // This filter is Exponential Moving Average.
        Landmark Filter(Landmark prev, Landmark current, float lerpAmount)
        {
            return Landmark.Lerp(prev, current, lerpAmount);
        }

        void UpdatePrevResults(ILandmarks landmarks)
        {
            CollectionUtils<Landmark>.ExpandList(landmarks.Landmarks.Count, ref _prevResults);
            _prevResults = landmarks.Landmarks;
        }
    }
}
