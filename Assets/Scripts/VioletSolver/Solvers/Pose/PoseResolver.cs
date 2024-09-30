// Copyright (c) 2024 HaruCoded
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using UnityEngine;
using HolisticPose;

namespace VioletSolver
{
	public class PoseResolver {
		public static DataGroups.PoseData SolvePose(Vector3[] landmarks) {
			DataGroups.PoseData pose = new();

			{
				pose.rShoulder = landmarks[(int)PoseLandmark.RightShoulder];
				pose.lShoulder = landmarks[(int)PoseLandmark.LeftShoulder];
				Vector4 rHip = landmarks[(int)PoseLandmark.LeftShoulder];
				Vector4 lHip = landmarks[(int)PoseLandmark.LeftShoulder];

                {
					Vector3 vRigA = Vector3.left;
					Vector3 vRigB = pose.rShoulder - pose.lShoulder;
					Quaternion rot = Quaternion.FromToRotation(vRigA, vRigB);
					pose.chestRotation = rot;

					vRigA = Vector3.left;
					vRigB = rHip - lHip;
					rot = Quaternion.FromToRotation(vRigA, vRigB);
					pose.hipsRotation = rot;

					float mul = 1000;
					pose.hipsPosition = new Vector3(
						(rHip.y + lHip.y) * 0.5f * mul,
						0, // -(rHip.z + lHip.z) * 0.5f * mul,
						0 // (rHip.y + lHip.y) * 0.5f * mul
					);
					/*
					float bodyRotation = 1.0f;
					bodyRotation = Mathf.Abs(Mathf.Cos(rot.eulerAngles.y * 1.6f));
					*/
				}

				{
					pose.rElbow = landmarks[(int)PoseLandmark.RightElbow];
					pose.rHand = landmarks[(int)PoseLandmark.RightWrist];
					Vector3 vRigB = pose.rElbow - pose.rShoulder;

					//if (pose.rHand.w < Settings.HandTrackingThreshold) {
					//	pose.rHand = (Vector3) pose.rElbow + vRigB;
					//}
				}

				{
					pose.lElbow = landmarks[(int)PoseLandmark.LeftElbow];
					pose.lHand = landmarks[(int)PoseLandmark.LeftWrist];
					Vector3 vRigB = pose.lElbow - pose.lShoulder;

					//if (pose.lHand.w < Settings.HandTrackingThreshold) {
					//	pose.lHand = (Vector3) pose.lElbow + vRigB;
					//}
				}

				// Legs
				{
					Vector4 rKnee = landmarks[(int)PoseLandmark.RightKnee];
					Vector4 rAnkle = landmarks[(int)PoseLandmark.RightAnkle];
					Vector3 vRigA = Vector3.up;
					Vector3 vRigB = rKnee - rHip;
					Quaternion rot = Quaternion.FromToRotation(vRigA, vRigB);
					pose.rUpperLeg = rot;

					Vector3 vRigC = rAnkle - rKnee;
					rot = Quaternion.FromToRotation(vRigA, vRigC);
					pose.rLowerLeg = rot;
					
					pose.hasRightLeg = rHip.w > 0.5 && rKnee.w > 0.5;
				}

				{
					Vector4 lKnee = landmarks[(int)PoseLandmark.LeftKnee];
					Vector4 lAnkle = landmarks[(int)PoseLandmark.LeftAnkle];
					Vector3 vRigA = Vector3.up;
					Vector3 vRigB = lKnee - lHip;
					Quaternion rot = Quaternion.FromToRotation(vRigA, vRigB);
					pose.lUpperLeg = rot;

					Vector3 vRigC = lAnkle - lKnee;
					rot = Quaternion.FromToRotation(vRigA, vRigC);
					pose.lLowerLeg = rot;

					pose.hasLeftLeg = lHip.w > 0.5 && lKnee.w > 0.5;
				}
			}

			return pose;
		}
	}
}

