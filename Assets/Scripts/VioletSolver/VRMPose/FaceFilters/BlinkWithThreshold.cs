using System.Collections.Generic;
using VRM;

namespace VioletSolver.Blendshapes
{
    public class EyeOpener : IBlendshapeFilter
    {
        float _threshold;
        public EyeOpener(float threshold) 
        { 
            _threshold = threshold;
        }
        public Dictionary<BlendShapePreset, float> Filter(Dictionary<BlendShapePreset, float> blendshapes, float amount)
        {
            var result = blendshapes;

            if (blendshapes[BlendShapePreset.Blink_L] < _threshold)
                result[BlendShapePreset.Blink_L] = 0f;
            if (blendshapes[BlendShapePreset.Blink_R] < _threshold)
                result[BlendShapePreset.Blink_R] = 0f;

            return result;
        }
    }

    public class EyeCloser : IBlendshapeFilter
    {
        float _threshold;
        public EyeCloser(float threshold)
        {
            _threshold = threshold;
        }
        public Dictionary<BlendShapePreset, float> Filter(Dictionary<BlendShapePreset, float> blendshapes, float amount)
        {
            var result = blendshapes;

            if (blendshapes[BlendShapePreset.Blink_L] > _threshold)
                result[BlendShapePreset.Blink_L] = 1f;
            if (blendshapes[BlendShapePreset.Blink_R] > _threshold)
                result[BlendShapePreset.Blink_R] = 1f;

            return result;
        }
    }
}
