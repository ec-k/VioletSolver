using System;
using System.Collections.Generic;
using System.Linq;
using UniVRM10;
using VRM;
using MediaPipeBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver
{
    /// <summary>
    /// Applies blendshapes to VRM 1.0 format avatars.
    /// </summary>
    public class Vrm10FaceApplier : IFaceApplier
    {
        readonly Vrm10RuntimeExpression _expression;

        /// <summary>
        /// Initializes a FaceApplier for VRM 1.0.
        /// </summary>
        /// <param name="expression">The Vrm10RuntimeExpression component.</param>
        public Vrm10FaceApplier(Vrm10RuntimeExpression expression) => _expression = expression;

        /// <inheritdoc/>
        public void Apply(IReadOnlyDictionary<BlendShapePreset, float> blendshapes)
        {
            var expressionDictionary = Enumerable.Range(0, Enum.GetValues(typeof(BlendShapePreset)).Length)
                .Select(i =>
                {
                    var vrm0xKey = (BlendShapePreset)i;

                    // NOTE: This process can NOT handle new expression "surprised".
                    var vrm10Key = vrm0xKey switch
                    {
                        BlendShapePreset.Joy => ExpressionPreset.happy,
                        BlendShapePreset.Angry => ExpressionPreset.angry,
                        BlendShapePreset.Sorrow => ExpressionPreset.sad,
                        BlendShapePreset.Fun => ExpressionPreset.relaxed,
                        BlendShapePreset.A => ExpressionPreset.aa,
                        BlendShapePreset.I => ExpressionPreset.ih,
                        BlendShapePreset.U => ExpressionPreset.ou,
                        BlendShapePreset.E => ExpressionPreset.ee,
                        BlendShapePreset.O => ExpressionPreset.oh,
                        _ => ExpressionPreset.custom
                    };

                    return new KeyValuePair<ExpressionKey, float>
                    (
                        ExpressionKey.CreateFromPreset(vrm10Key),
                        blendshapes[(BlendShapePreset)i]
                    );
                })
                .ToDictionary(
                    item => item.Key,
                    item => item.Value
                );

            foreach (var kvp in expressionDictionary)
                _expression.SetWeight(kvp.Key, kvp.Value);
        }

        /// <inheritdoc/>
        public void Apply(IReadOnlyDictionary<MediaPipeBlendshapes, float> blendshapes)
        {
            var expressionDictionary = Enumerable.Range(0, (int)MediaPipeBlendshapes.Length)
                .Select(i => (MediaPipeBlendshapes)i)
                .Where(mpKey => blendshapes.ContainsKey(mpKey))
                .ToDictionary(
                    mpKey => ExpressionKey.CreateCustom(Enum.GetName(typeof(MediaPipeBlendshapes), mpKey)),
                    mpKey => blendshapes[mpKey]
                );

            foreach (var kvp in expressionDictionary)
                _expression.SetWeight(kvp.Key, kvp.Value);
        }
    }
}
