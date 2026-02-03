using System;
using System.Collections.Generic;
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
            var presetCount = Enum.GetValues(typeof(BlendShapePreset)).Length;
            for (var i = 0; i < presetCount; i++)
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

                _expression.SetWeight(
                    ExpressionKey.CreateFromPreset(vrm10Key),
                    blendshapes[vrm0xKey]);
            }
        }

        /// <inheritdoc/>
        public void Apply(IReadOnlyDictionary<MediaPipeBlendshapes, float> blendshapes)
        {
            for (var i = 0; i < (int)MediaPipeBlendshapes.Length; i++)
            {
                var mpKey = (MediaPipeBlendshapes)i;
                if (!blendshapes.ContainsKey(mpKey)) continue;

                var expressionKey = ExpressionKey.CreateCustom(Enum.GetName(typeof(MediaPipeBlendshapes), mpKey));
                _expression.SetWeight(expressionKey, blendshapes[mpKey]);
            }
        }
    }
}
