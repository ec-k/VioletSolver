// Copyright (c) 2024 HaruCoded
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using UnityEngine;
using HolisticPose;
using System.Collections.Generic;

namespace VioletSolver
{
	public class PoseSolver
	{
		public static AvatarPoseData SolvePose(List<Landmark> landmarks)
		{
            AvatarPoseData pose = new();

			{
				Vector3 rShoulder = landmarks[(int)PoseLandmark.RightShoulder];
				Vector3 lShoulder = landmarks[(int)PoseLandmark.LeftShoulder];
				Vector3 rHip = landmarks[(int)PoseLandmark.LeftShoulder];
				Vector3 lHip = landmarks[(int)PoseLandmark.LeftShoulder];

				// Hips & Chest
				{
					Vector3 defaultDirection = Vector3.left;
					Vector3 targetDirection = rShoulder - lShoulder;
					Quaternion rot = Quaternion.FromToRotation(defaultDirection, targetDirection);
					pose.Chest= rot;

					defaultDirection = Vector3.left;
					targetDirection = rHip - lHip;
					rot = Quaternion.FromToRotation(defaultDirection, targetDirection);
					pose.Hips = rot;

					float mul = 1000;
					pose.HipsPosition = new Vector3(
						(rHip.y + lHip.y) * 0.5f * mul,
						0, // -(rHip.z + lHip.z) * 0.5f * mul,
						0 // (rHip.y + lHip.y) * 0.5f * mul
					);
					/*
					float bodyRotation = 1.0f;
					bodyRotation = Mathf.Abs(Mathf.Cos(rot.eulerAngles.y * 1.6f));
					*/
				}

				// Arms
				{
					Vector4 rElbow = landmarks[(int)PoseLandmark.RightElbow];
					Vector4 rHand = landmarks[(int)PoseLandmark.RightWrist];
					(pose.RightUpperArm, pose.RightLowerArm) = SolveRim(rShoulder, rElbow, rHand, Vector3.up);
				}

				{
					Vector4 lElbow = landmarks[(int)PoseLandmark.LeftElbow];
					Vector4 lHand = landmarks[(int)PoseLandmark.LeftWrist];
					(pose.LeftUpperArm, pose.LeftLowerArm) = SolveRim(lShoulder, lElbow, lHand, Vector3.up);
				}

				// Legs
				{
					Vector4 rKnee = landmarks[(int)PoseLandmark.RightKnee];
					Vector4 rAnkle = landmarks[(int)PoseLandmark.RightAnkle];
					(pose.RightUpperLeg, pose.RightLowerLeg) = SolveRim(rHip, rKnee, rAnkle, Vector3.up);

					//pose.hasRightLeg = rHip.w > 0.5 && rKnee.w > 0.5;
				}

				{
					Vector4 lKnee = landmarks[(int)PoseLandmark.LeftKnee];
					Vector4 lAnkle = landmarks[(int)PoseLandmark.LeftAnkle];
                    (pose.LeftUpperLeg, pose.LeftLowerLeg) = SolveRim(lHip, lKnee, lAnkle, Vector3.up);

					//pose.hasLeftLeg = lHip.w > 0.5 && lKnee.w > 0.5;
				}
			}

			return pose;
		}

		static (Quaternion, Quaternion) SolveRim(Vector3 root, Vector3 mid, Vector3 tip, Vector3 baseDirection)
		{
			var (rootRot, midRot) = (Quaternion.identity, Quaternion.identity);

			rootRot = Quaternion.FromToRotation(baseDirection, mid - root);
			midRot = Quaternion.FromToRotation(baseDirection, tip - mid);

			return (rootRot, midRot);
		}
	}
}

