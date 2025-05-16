using System.Collections.Generic;

namespace VioletSolver.Landmarks
{
    internal class SingleExponentialSmoothingFilter : ILandmarkFilter
    {
        public List<Landmark> PrevResults { get; private set; }
        float _smoothingFactor;
        bool _isFirst = true;

        internal SingleExponentialSmoothingFilter(int landmarkLength, float smootingFactor)
        {
            _smoothingFactor = smootingFactor;
            PrevResults = new List<Landmark>(landmarkLength);
            for (var i = 0; i < landmarkLength; i++)
                PrevResults.Add(new Landmark());
        }
        internal SingleExponentialSmoothingFilter(int landmarkLength)
        {
            _smoothingFactor = 0f;
            PrevResults = new List<Landmark>(landmarkLength);
            for (var i = 0; i < landmarkLength; i++)
                PrevResults.Add(new Landmark());
        }

        public ILandmarks Filter(ILandmarks landmarks)
        {
            if (_isFirst)
                _isFirst = false;
            else
            {
                // Implicitly, landmarks.Count = _prevResults.Count is assumed.
                for (var i = 0; i < landmarks.Count; i++)
                {
                    landmarks.Landmarks[i] = Filter(PrevResults[i], landmarks.Landmarks[i], _smoothingFactor);
                }
            }

            UpdatePrevResults(landmarks);

            return landmarks;
        }

        public ILandmarks Filter(ILandmarks landmarks, float lerpAmount)
        {
            if (_isFirst)
                _isFirst = false;
            else
            {
                // Implicitly, landmarks.Count = _prevResults.Count is assumed.
                for (var i = 0; i < landmarks.Count; i++)
                {
                    landmarks.Landmarks[i] = Filter(PrevResults[i], landmarks.Landmarks[i], lerpAmount);
                }
            }

            UpdatePrevResults(landmarks);

            return landmarks;
        }

        public Landmark Filter(Landmark prev, Landmark current, float lerpAmount)
        {
            return Landmark.Lerp(prev, current, lerpAmount);
        }

        void UpdatePrevResults(in ILandmarks landmarks)
        {
            for (var i = 0;i< PrevResults.Count; i++)
            {
                PrevResults[i] = landmarks.Landmarks[i];
            }
        }

        public void UpdatePrevResult(Landmark result, int index)
        {
            PrevResults[index] = result;
        }
    }
}
