using System.Collections.Generic;
using UnityEngine;
using VioletSolver.Landmarks;

namespace VioletSolver
{
    internal class OneEuroFilter : ILandmarkFilter
    {
        List<Landmark> _prevResults;
        float _prevTime = 0f;
        float _minCutoff;
        float _slope;
        float _dCutoff;
        SingleExponentialSmoothingFilter _xFilter;
        SingleExponentialSmoothingFilter _dxFilter;

        bool _isFirst = true;

        internal OneEuroFilter(int resultLength, float minCutoff, float slope, float dCutoff)
        {
            _minCutoff = minCutoff;
            _slope = slope;
            _dCutoff = dCutoff;
            _xFilter = new(resultLength);
            _dxFilter = new(resultLength);
            _prevResults = new List<Landmark>(resultLength);
            for (var i = 0; i < resultLength; i++)
                _prevResults.Add(new Landmark());
        }

        public ILandmarks Filter(ILandmarks current)
        {
            var t_e = current.Time - _prevTime;

            // Do nothing if the time difference is too small.
            if (t_e < 1e-5f)
            {
                for (var i = 0; i < current.Count; i++)
                    current.Landmarks[i] = _prevResults[i];
                return current;
            }

            var updateRate = 1f / t_e;
            for(var i = 0; i < current.Count; i++)
            {
                current.Landmarks[i] = Filter(current.Landmarks[i], i, updateRate);
            }

            UpdatePrevResults(current);

            return current;
        }

        Landmark Filter(Landmark current, int index, float updateRate)
        {
            var dx = new Landmark(Vector3.zero, 1f);
            if (_isFirst)
                _isFirst = false;
            else
            {
                dx = (current - _xFilter.PrevResults[index]) * updateRate;
            }
            var edx = _dxFilter.Filter(dx, _dxFilter.PrevResults[index], Alpha(updateRate, _dCutoff));
            _dxFilter.UpdatePrevResult(edx, index); // HACK: This should do in LPF's private method.
            var cutoff = _minCutoff + _slope * edx.Position.magnitude;

            var alpha = Alpha(updateRate, cutoff);
            return _xFilter.Filter(_prevResults[index], current, alpha);
        }

        float Alpha(float updateRate, float cutoffFrequency)
        {
            var timeConstant = 1f / (2 * Mathf.PI * cutoffFrequency);
            return 1f / (1f + timeConstant * updateRate);
        }

        void UpdatePrevResults(in ILandmarks landmarks)
        {
            _prevTime = landmarks.Time;
            for (var i = 0; i < _prevResults.Count; i++)
            {
                _prevResults[i] = landmarks.Landmarks[i];
            }
        }
    }
}
