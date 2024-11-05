// Copyright (c) 2024 HaruCoded
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using UnityEngine;
using System.Collections.Generic;
using VRM;

using blendshapeIndex = HolisticPose.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver
{
	public class FaceResolver {
		// TODO: Later, change this function to return (Quaternion neckRotation, float[] blendshapes)
		public static Quaternion Solve(List<Landmark> landmarks)
		{
			Quaternion neckRotation = Quaternion.identity;
			//float mouthOpen = 0;

			//{
			//	// Mouth
			//	Vector3 a = landmarks[324];
			//	Vector3 b = landmarks[78];
			//	Vector3 c = landmarks[13];
			//	Vector3 m = (a + b) / 2.0f;

			//	float width = Vector3.Distance(a, b);
			//	float height = Vector3.Distance(c, m);
			//	//float area = MovementUtils.GetTriangleArea(a, b, c);
			//	float perc = height / width;

			//	mouthOpen = (perc - 0.25f) * 3;
			//	mouthOpen = Mathf.Clamp01(mouthOpen);

			{
				Vector3 botHead = landmarks[152];
				Vector3 topHead = landmarks[10];
				Plane plane = new(landmarks[109], landmarks[338], botHead);

				// Figure out their position on the eye socket plane
				Vector3 forwardDir = plane.normal;
				Vector3 faceUpDir = botHead - topHead;

				neckRotation = Quaternion.LookRotation(forwardDir, faceUpDir);
			}

			//data.mouthOpen = mouthOpen;
			//data.rEyeOpen = rEyeOpen;
			//data.lEyeOpen = lEyeOpen;
			//data.rEyeIris = rEyeIris;
			//data.lEyeIris = lEyeIris;

			return neckRotation;
		}

        public static Dictionary<BlendShapePreset, float> SolveFacialExpression(Dictionary<blendshapeIndex, float> mp_blendshapes)
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
        public static Dictionary<blendshapeIndex, float> SolveFacialExpressionPerfectly(Dictionary<blendshapeIndex, float> mp_blendshapes)
        {
            return mp_blendshapes;
        }

        public static (Quaternion, Quaternion) SolveEye(Dictionary<blendshapeIndex, float> mp_blendshapes)
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

