// Copyright (c) 2024 HaruCoded
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using UnityEngine;
using System.Collections.Generic;

namespace VioletSolver
{
	public class FaceResolver {
		// TODO: Later, change this function to return (Quaternion neckRotation, float[] blendshapes)
		public static Quaternion Solve(List<Landmark> landmarks)
		{
			Quaternion neckRotation = Quaternion.identity;
			//float mouthOpen = 0;
			//float lEyeOpen = 0;
			//float rEyeOpen = 0;
			//Vector2 lEyeIris = Vector2.zero;
			//Vector2 rEyeIris = Vector2.zero;

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

			//	Vector3 converter(int i)
			//	{
			//		Vector3 value = landmarks[i];
			//		value.x = -value.x;
			//		return value;
			//	}

			//	// Eyes
			//	lEyeOpen = FacePoints.CalculateEyeAspectRatio(Array.ConvertAll(FacePoints.LeftEyeEAR, converter));
			//	rEyeOpen = FacePoints.CalculateEyeAspectRatio(Array.ConvertAll(FacePoints.RightEyeEAR, converter));
			//	lEyeIris = FacePoints.CalculateIrisPosition(FacePoints.LeftEyeIrisPoint, converter);
			//	rEyeIris = FacePoints.CalculateIrisPosition(FacePoints.RightEyeIrisPoint, converter);
			//}

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
	}
}

