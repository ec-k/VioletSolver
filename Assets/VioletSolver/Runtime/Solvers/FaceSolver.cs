// Copyright (c) 2024 HaruCoded
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using UnityEngine;
using System.Collections.Generic;
using VRM;
using VioletSolver.Landmarks;
using blendshapeIndex = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver.Solver
{
	internal class FaceSolver {
		// TODO: Later, change this function to return (Quaternion neckRotation, float[] blendshapes)
		internal static Quaternion Solve(in IReadOnlyList<Landmark> landmarks)
		{
			Quaternion neckRotation = Quaternion.identity;
			{
				Vector3 botHead = landmarks[152].Position;
				Vector3 topHead = landmarks[10].Position;
				Plane plane = new(landmarks[109].Position, landmarks[338].Position, botHead);

				// Figure out their position on the eye socket plane
				Vector3 forwardDir = plane.normal;
				Vector3 faceUpDir = botHead - topHead;

				if(forwardDir == Vector3.zero || faceUpDir == Vector3.zero)
					neckRotation = Quaternion.identity;
				else
					neckRotation = Quaternion.LookRotation(forwardDir, faceUpDir);
			}
			return neckRotation;
		}

        internal static Dictionary<BlendShapePreset, float> SolveFacialExpression(Dictionary<blendshapeIndex, float> mp_blendshapes)
		{
			var weights = new Dictionary<BlendShapePreset, float>();

			// blink
			weights[BlendShapePreset.Blink_L] = mp_blendshapes[blendshapeIndex.EyeBlinkLeft];
			weights[BlendShapePreset.Blink_R] = mp_blendshapes[blendshapeIndex.EyeBlinkRight];

			// viseme
			weights[BlendShapePreset.O] = mp_blendshapes[blendshapeIndex.JawOpen];
			weights[BlendShapePreset.U] = mp_blendshapes[blendshapeIndex.MouthFunnel];
			weights[BlendShapePreset.I] = mp_blendshapes[blendshapeIndex.MouthDimpleLeft] * 0.4f + mp_blendshapes[blendshapeIndex.MouthDimpleRight] * 0.4f + mp_blendshapes[blendshapeIndex.JawOpen] * 0.2f;

			return weights;
		}

        /// <summary>
        ///		Solve face for Perfect Sync.
        /// </summary>
        /// <param name="mp_blendshapes"></param>
        /// <returns></returns>
        // NOTE: Mediapipe Blendshapes' parameters are same as Apple ARkit Blendshapes.
        internal static Dictionary<blendshapeIndex, float> SolveFacialExpressionPerfectly(Dictionary<blendshapeIndex, float> mp_blendshapes)
        {
            return mp_blendshapes;
        }

        internal static (Quaternion, Quaternion) SolveEye(Dictionary<blendshapeIndex, float> mp_blendshapes)
		{
            var weights = new Dictionary<BlendShapePreset, float>();
			var (leftEye, rightEye) = (Quaternion.identity, Quaternion.identity);

			var maxRotation = 10f;

			// left eye
			{
				var up = mp_blendshapes[blendshapeIndex.EyeLookUpLeft];
				var down = mp_blendshapes[blendshapeIndex.EyeLookDownLeft];
				var left = mp_blendshapes[blendshapeIndex.EyeLookOutLeft];
				var right = mp_blendshapes[blendshapeIndex.EyeLookInLeft];

				var x_value = down - up;
				var y_value = right - left;

				var x_normalized = (x_value + 1f) / 2f;
				var y_normalized = (y_value + 1f) / 2f;

                var rot_x = Mathf.Lerp(-maxRotation, maxRotation, x_normalized);
				var rot_y = Mathf.Lerp(-maxRotation, maxRotation, y_normalized);

                leftEye = Quaternion.Euler(rot_x, rot_y, 0f);
            }

            // right eye
            {
				var up = mp_blendshapes[blendshapeIndex.EyeLookUpRight];
                var down = mp_blendshapes[blendshapeIndex.EyeLookDownRight];
                var left = mp_blendshapes[blendshapeIndex.EyeLookInRight];
                var right = mp_blendshapes[blendshapeIndex.EyeLookOutRight];

                var x_value = down - up;
                var y_value = right - left;

                var x_normalized = (x_value + 1f) / 2f;
                var y_normalized = (y_value + 1f) / 2f;

                var rot_x = Mathf.Lerp(-maxRotation, maxRotation, x_normalized);
                var rot_y = Mathf.Lerp(-maxRotation, maxRotation, y_normalized);

                rightEye = Quaternion.Euler(rot_x, rot_y, 0f);
            }

			return (leftEye, rightEye);

		}

    }
}

