using System.Collections.Generic;
using UnityEngine;

namespace VioletSolver
{
    public class BlendshapeInterpolator<TKey>
    {
        Dictionary<TKey, float> _prevBlendshapes = new();
        Dictionary<TKey, float> _nextBlendshapes = new();
        Dictionary<TKey, float> _result = new();
        float _lastProcessedDataTime; // Time of the _nextBlendshapes data
        const float _dataInterval = 1f / 30f; // Assuming 30Hz blendshape data

        public BlendshapeInterpolator()
        {
            _lastProcessedDataTime = 0f;
        }

        public Dictionary<TKey, float> UpdateAndInterpolate(IReadOnlyDictionary<TKey, float> newBlendshapes, float newDataTime)
        {
            // Update buffers
            try
            {
                DictionaryValueCopy(_nextBlendshapes, _prevBlendshapes);
                DictionaryValueCopy(newBlendshapes, _nextBlendshapes);
                _lastProcessedDataTime = newDataTime;
            }
            catch (System.InvalidOperationException)
            {
                // Skip updating for this frame if a collection modification conflict occurred.
                Debug.LogWarning("[BlendshapeInterpolator] Source collection was modified during copy. Skipping update until the next frame.");
                return _result;
            }

            float interpolationAmount = Mathf.Clamp01((Time.time - _lastProcessedDataTime) / _dataInterval);

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