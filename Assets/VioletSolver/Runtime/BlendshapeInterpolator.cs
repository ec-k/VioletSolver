using System.Collections.Generic;
using UnityEngine;

namespace VioletSolver
{
    public class BlendshapeInterpolator<TKey>
    {
        Dictionary<TKey, float> _prevBlendshapes;
        Dictionary<TKey, float> _nextBlendshapes;
        float _lastProcessedDataTime; // Time of the _nextBlendshapes data
        const float _dataInterval = 1f / 30f; // Assuming 30Hz blendshape data

        public BlendshapeInterpolator()
        {
            _prevBlendshapes = new();
            _nextBlendshapes = new();
            _lastProcessedDataTime = 0f;
        }

        public Dictionary<TKey, float> UpdateAndInterpolate(IReadOnlyDictionary<TKey, float> newBlendshapes, float newDataTime)
        {
            if (newDataTime > _lastProcessedDataTime)
            {
                _prevBlendshapes = new(_nextBlendshapes); // Copy current next to prev
                _nextBlendshapes = new(newBlendshapes);   // Update next with new data
                _lastProcessedDataTime = newDataTime;
            }

            float interpolationAmount = Mathf.Clamp01((Time.time - _lastProcessedDataTime) / _dataInterval);

            Dictionary<TKey, float> result = new Dictionary<TKey, float>();

            // Interpolate each blendshape value
            foreach (var key in _nextBlendshapes.Keys)
            {
                float prevValue = _prevBlendshapes.ContainsKey(key) ? _prevBlendshapes[key] : 0f;
                float nextValue = _nextBlendshapes[key];
                result[key] = Mathf.Lerp(prevValue, nextValue, interpolationAmount);
            }

            return result;
        }
    }
}