using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using VRM;

namespace VioletSolver.Blendshapes
{
    public class LowPassFilter : IBlendshapeFilter
    {
        Dictionary<BlendShapePreset, float> _prevWeights;
        float _amount;
        public LowPassFilter(float amount)
        {
            _amount = amount;
            _prevWeights = new();
        }

        public Dictionary<BlendShapePreset, float> Filter(Dictionary<BlendShapePreset, float> blendshapes)
        {
            var results = blendshapes;

            var bss = Enum.GetValues(typeof(BlendShapePreset));
            foreach (var bs in bss)
            {
                var blendshapeIndex = (BlendShapePreset)bs;
                try
                {
                    results[blendshapeIndex] = Mathf.Lerp(_prevWeights[blendshapeIndex], blendshapes[blendshapeIndex], _amount);
                }
                catch { }
            }

            _prevWeights = results;
            return results;
        }
    }
}
