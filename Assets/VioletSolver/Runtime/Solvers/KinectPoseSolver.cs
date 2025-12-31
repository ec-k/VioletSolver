// Copyright (c) 2024 HaruCoded
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using UnityEngine;
using System.Collections.Generic;

using VioletSolver.Landmarks;
using VioletSolver.Pose;
using poseIndex = HumanLandmarks.KinectPoseLandmarks.Types.LandmarkIndex;

namespace VioletSolver.Solver
{
	internal class KinectPoseSolver
	{
		internal static AvatarPoseData SolvePose(in IReadOnlyList<Landmark> landmarks, AvatarBonePositions restBonePositions, bool useIk)
		{
            AvatarPoseData pose = new();

			{
				Vector3 rShoulder = landmarks[(int)poseIndex.ShoulderRight].Position;
				Vector3 lShoulder = landmarks[(int)poseIndex.ShoulderLeft].Position;
				Vector3 rHip = landmarks[(int)poseIndex.HipRight].Position;
				Vector3 lHip = landmarks[(int)poseIndex.HipLeft].Position;

				// Hips & Chest
				{
                    Vector3 defaultDirection = restBonePositions.UpperChest - restBonePositions.Hips;
                    Vector3 targetDirection = Vector3.Lerp(rShoulder, lShoulder, 0.5f) - Vector3.Lerp(rHip, lHip, 0.5f);
                    var tiltRot = Quaternion.FromToRotation(defaultDirection, targetDirection);

                    defaultDirection = restBonePositions.RightShoulder - restBonePositions.LeftShoulder;
					targetDirection = rShoulder - lShoulder;
					var rot = Quaternion.FromToRotation(defaultDirection, targetDirection);
                    pose.Chest = rot * tiltRot;

					targetDirection = rHip - lHip;
					rot = Quaternion.FromToRotation(defaultDirection, targetDirection);
					pose.Hips = rot * tiltRot;

					float mul = 1;
					pose.HipsPosition = new Vector3(
						(rHip.x + lHip.x) * 0.5f * mul,
						(rHip.y + lHip.y) * 0.5f * mul,
						(rHip.z + lHip.z) * 0.5f * mul
					);
					/*
					float bodyRotation = 1.0f;
					bodyRotation = Mathf.Abs(Mathf.Cos(rot.eulerAngles.y * 1.6f));
					*/
				}

				if (useIk)
					SolveIkTargets(landmarks, ref pose);
				else
				{
					// Arms
					{
						Vector4 rElbow = landmarks[(int)poseIndex.ElbowRight].Position;
						Vector4 rHand = landmarks[(int)poseIndex.WristRight].Position;
						var baseDirection = restBonePositions.RightLowerArm - restBonePositions.RightUpperArm;
						(pose.RightUpperArm, pose.RightLowerArm) = SolveRim(rShoulder, rElbow, rHand, baseDirection);
					}

					{
						Vector4 lElbow = landmarks[(int)poseIndex.ElbowLeft].Position;
						Vector4 lHand = landmarks[(int)poseIndex.WristLeft].Position;
						var baseDirection = restBonePositions.LeftLowerArm - restBonePositions.LeftUpperArm;
						(pose.LeftUpperArm, pose.LeftLowerArm) = SolveRim(lShoulder, lElbow, lHand, baseDirection);
					}
				}

				// Legs
				{
					Vector4 rKnee = landmarks[(int)poseIndex.KneeRight].Position;
					Vector4 rAnkle = landmarks[(int)poseIndex.AnkleRight].Position;
                    var baseDirection = restBonePositions.RightLowerLeg - restBonePositions.RightUpperLeg;
                    (pose.RightUpperLeg, pose.RightLowerLeg) = SolveRim(rHip, rKnee, rAnkle, baseDirection);

					//pose.hasRightLeg = rHip.w > 0.5 && rKnee.w > 0.5;
				}

				{
					Vector4 lKnee = landmarks[(int)poseIndex.KneeLeft].Position;
					Vector4 lAnkle = landmarks[(int)poseIndex.AnkleLeft].Position;
                    var baseDirection = restBonePositions.LeftLowerLeg - restBonePositions.LeftUpperLeg;
                    (pose.LeftUpperLeg, pose.LeftLowerLeg) = SolveRim(lHip, lKnee, lAnkle, baseDirection);

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

        static void SolveIkTargets(in IReadOnlyList<Landmark> landmarks, ref AvatarPoseData pose)
        {
			pose.HeadPosition = landmarks[(int)poseIndex.Nose].Position;

            pose.LeftHandPosition = landmarks[(int)poseIndex.WristLeft].Position;
            pose.LeftElbowPosition = landmarks[(int)poseIndex.ElbowLeft].Position;
            pose.LeftShoulderPosition = landmarks[(int)poseIndex.ShoulderLeft].Position;

            pose.RightHandPosition = landmarks[(int)poseIndex.WristRight].Position;
            pose.RightElbowPosition = landmarks[(int)poseIndex.ElbowRight].Position;
            pose.RightShoulderPosition = landmarks[(int)poseIndex.ShoulderRight].Position;

			pose.LeftThighPosition = landmarks[(int)poseIndex.HipLeft].Position;
			pose.LeftKneePosition = landmarks[(int)poseIndex.KneeLeft].Position;
			pose.LeftFootPosition = landmarks[(int)poseIndex.AnkleLeft].Position;

            pose.RightThighPosition = landmarks[(int)poseIndex.HipRight].Position;
            pose.RightKneePosition = landmarks[(int)poseIndex.KneeRight].Position;
            pose.RightFootPosition = landmarks[(int)poseIndex.AnkleRight].Position;
        }
    }
}

