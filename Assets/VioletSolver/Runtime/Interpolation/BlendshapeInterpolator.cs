using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace VioletSolver.Interpolation
{
    public class BlendshapeInterpolator<TKey>
    {
        Dictionary<TKey, float> _prevBlendshapes = new();
        Dictionary<TKey, float> _nextBlendshapes = new();
        Dictionary<TKey, float> _result = new();
        float _prevDataTime;
        float _lastDataTime;
        readonly float _fixedDataInterval;

        public bool UseVariableFrameRate { get; set; }

        public BlendshapeInterpolator(bool useVariableFrameRate = false, int dataFrameRate = 30)
        {
            UseVariableFrameRate = useVariableFrameRate;
            _fixedDataInterval = 1f / dataFrameRate;
            _prevDataTime = 0f;
            _lastDataTime = 0f;
        }

        public Dictionary<TKey, float> UpdateAndInterpolate(IReadOnlyDictionary<TKey, float> newBlendshapes, float newDataTime)
        {
            // Update buffers
            try
            {
                DictionaryValueCopy(_nextBlendshapes, _prevBlendshapes);
                DictionaryValueCopy(newBlendshapes, _nextBlendshapes);
                _prevDataTime = _lastDataTime;
                _lastDataTime = newDataTime;
            }
            catch (System.InvalidOperationException)
            {
                // Skip updating for this frame if a collection modification conflict occurred.
                UnityEngine.Debug.LogWarning("[BlendshapeInterpolator] Source collection was modified during copy. Skipping update until the next frame.");
                return _result;
            }

            float currentTime = (float)Stopwatch.GetTimestamp() / Stopwatch.Frequency;
            float dataInterval = UseVariableFrameRate && _prevDataTime > 0f
                ? _lastDataTime - _prevDataTime
                : _fixedDataInterval;
            float interpolationAmount = Mathf.Clamp01((currentTime - _prevDataTime) / dataInterval);

            _result.Clear();

            // Interpolate each blendshape value
            foreach (var kvp in _nextBlendshapes)
            {
                _prevBlendshapes.TryGetValue(kvp.Key, out var prevValue);
                var nextValue = kvp.Value;

                _result[kvp.Key] = Mathf.Lerp(prevValue, nextValue, interpolationAmount);
            }

            return _result;
        }

        void DictionaryValueCopy(IReadOnlyDictionary<TKey, float> source, Dictionary<TKey, float> destination)
        {
            if (source is null) return;

            destination.Clear();
            foreach (var kvp in source)
            {
                destination[kvp.Key] = kvp.Value;
            }
        }
    }
}