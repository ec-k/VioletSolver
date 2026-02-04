using System.Collections.Generic;
using UnityEngine;
using VRM;
using MediaPipeBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver.Solver.Face
{
    /// <summary>
    /// Solver that converts MediaPipe blendshapes to VRM BlendShapePresets using the standard method.
    /// </summary>
    public class StandardBlendshapeSolver : IBlendshapeSolver
    {
        const float MaxEyeRotation = 10f;

        /// <inheritdoc/>
        public BlendshapeResult Solve(IReadOnlyDictionary<MediaPipeBlendshapes, float> weights)
        {
            if (weights == null || weights.Count <= 0)
                return null;

            var blendshapes = SolveFacialExpression(weights);
            var (leftEye, rightEye) = SolveEye(weights);

            return new BlendshapeResult
            {
                VrmBlendshapes = blendshapes,
                LeftEye = leftEye,
                RightEye = rightEye,
            };
        }

        static Dictionary<BlendShapePreset, float> SolveFacialExpression(IReadOnlyDictionary<MediaPipeBlendshapes, float> weights)
        {
            var result = new Dictionary<BlendShapePreset, float>();

            // blink
            result[BlendShapePreset.Blink_L] = weights[MediaPipeBlendshapes.EyeBlinkLeft];
            result[BlendShapePreset.Blink_R] = weights[MediaPipeBlendshapes.EyeBlinkRight];

            // viseme
            result[BlendShapePreset.O] = weights[MediaPipeBlendshapes.JawOpen];
            result[BlendShapePreset.U] = weights[MediaPipeBlendshapes.MouthFunnel];
            result[BlendShapePreset.I] = weights[MediaPipeBlendshapes.MouthDimpleLeft] * 0.4f
                + weights[MediaPipeBlendshapes.MouthDimpleRight] * 0.4f
                + weights[MediaPipeBlendshapes.JawOpen] * 0.2f;

            return result;
        }

        static (Quaternion, Quaternion) SolveEye(IReadOnlyDictionary<MediaPipeBlendshapes, float> weights)
        {
            // left eye
            var leftUp = weights[MediaPipeBlendshapes.EyeLookUpLeft];
            var leftDown = weights[MediaPipeBlendshapes.EyeLookDownLeft];
            var leftOut = weights[MediaPipeBlendshapes.EyeLookOutLeft];
            var leftIn = weights[MediaPipeBlendshapes.EyeLookInLeft];

            var leftXNorm = (leftDown - leftUp + 1f) / 2f;
            var leftYNorm = (leftIn - leftOut + 1f) / 2f;
            var leftEye = Quaternion.Euler(
                Mathf.Lerp(-MaxEyeRotation, MaxEyeRotation, leftXNorm),
                Mathf.Lerp(-MaxEyeRotation, MaxEyeRotation, leftYNorm),
                0f);

            // right eye
            var rightUp = weights[MediaPipeBlendshapes.EyeLookUpRight];
            var rightDown = weights[MediaPipeBlendshapes.EyeLookDownRight];
            var rightIn = weights[MediaPipeBlendshapes.EyeLookInRight];
            var rightOut = weights[MediaPipeBlendshapes.EyeLookOutRight];

            var rightXNorm = (rightDown - rightUp + 1f) / 2f;
            var rightYNorm = (rightOut - rightIn + 1f) / 2f;
            var rightEye = Quaternion.Euler(
                Mathf.Lerp(-MaxEyeRotation, MaxEyeRotation, rightXNorm),
                Mathf.Lerp(-MaxEyeRotation, MaxEyeRotation, rightYNorm),
                0f);

            return (leftEye, rightEye);
        }
    }
}
