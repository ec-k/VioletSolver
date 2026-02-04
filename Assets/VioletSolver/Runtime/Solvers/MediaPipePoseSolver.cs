// Copyright (c) 2024 HaruCoded
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using UnityEngine;
using System.Collections.Generic;

using VioletSolver.Landmarks;
using VioletSolver.Pose;
using VioletSolver.Solvers.RestPose;
using poseIndex = HumanLandmarks.MediaPipePoseLandmarks.Types.LandmarkIndex;

namespace VioletSolver.Solver
{
	internal class MediaPipePoseSolver
	{
		internal static AvatarPoseData SolvePose(in IReadOnlyList<Landmark> landmarks, in AvatarBones restBones, bool useIk)
		{
            AvatarPoseData pose = new();

			{
				Vector3 rShoulder = landmarks[(int)poseIndex.RightShoulder].Position;
				Vector3 lShoulder = landmarks[(int)poseIndex.LeftShoulder].Position;
				Vector3 rHip = landmarks[(int)poseIndex.RightHip].Position;
				Vector3 lHip = landmarks[(int)poseIndex.LeftHip].Position;

				// Hips & Chest
				{
                    Vector3 defaultDirection = restBones.UpperChest.Position - restBones.Hips.Position;
                    Vector3 targetDirection = Vector3.Lerp(rShoulder, lShoulder, 0.5f) - Vector3.Lerp(rHip, lHip, 0.5f);
                    var tiltRot = Quaternion.FromToRotation(defaultDirection, targetDirection);

                    defaultDirection = restBones.RightShoulder.Position - restBones.LeftShoulder.Position;
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
				}

				if (useIk)
					SolveIkTargets(landmarks, ref pose);
				else
				{
					// Arms
					{
						Vector3 rElbow = landmarks[(int)poseIndex.RightElbow].Position;
						Vector3 rHand = landmarks[(int)poseIndex.RightWrist].Position;
						var baseDirection = restBones.RightLowerArm.Position - restBones.RightUpperArm.Position;
						(pose.RightUpperArm, pose.RightLowerArm) = SolveRim(rShoulder, rElbow, rHand, baseDirection);
					}

					{
						Vector3 lElbow = landmarks[(int)poseIndex.LeftElbow].Position;
						Vector3 lHand = landmarks[(int)poseIndex.LeftWrist].Position;
						var baseDirection = restBones.LeftLowerArm.Position - restBones.LeftUpperArm.Position;
						(pose.LeftUpperArm, pose.LeftLowerArm) = SolveRim(lShoulder, lElbow, lHand, baseDirection);
					}
				}

				// Legs
				{
					Vector3 rKnee = landmarks[(int)poseIndex.RightKnee].Position;
					Vector3 rAnkle = landmarks[(int)poseIndex.RightAnkle].Position;
                    var baseDirection = restBones.RightLowerLeg.Position - restBones.RightUpperLeg.Position;
                    (pose.RightUpperLeg, pose.RightLowerLeg) = SolveRim(rHip, rKnee, rAnkle, baseDirection);
				}

				{
					Vector3 lKnee = landmarks[(int)poseIndex.LeftKnee].Position;
					Vector3 lAnkle = landmarks[(int)poseIndex.LeftAnkle].Position;
                    var baseDirection = restBones.LeftLowerLeg.Position - restBones.LeftUpperLeg.Position;
                    (pose.LeftUpperLeg, pose.LeftLowerLeg) = SolveRim(lHip, lKnee, lAnkle, baseDirection);
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

            pose.LeftHandPosition = landmarks[(int)poseIndex.LeftWrist].Position;
            pose.LeftElbowPosition = landmarks[(int)poseIndex.LeftElbow].Position;
            pose.LeftShoulderPosition = landmarks[(int)poseIndex.LeftShoulder].Position;

            pose.RightHandPosition = landmarks[(int)poseIndex.RightWrist].Position;
            pose.RightElbowPosition = landmarks[(int)poseIndex.RightElbow].Position;
            pose.RightShoulderPosition = landmarks[(int)poseIndex.RightShoulder].Position;

			pose.LeftThighPosition = landmarks[(int)poseIndex.LeftHip].Position;
			pose.LeftKneePosition = landmarks[(int)poseIndex.LeftKnee].Position;
			pose.LeftFootPosition = landmarks[(int)poseIndex.LeftAnkle].Position;

            pose.RightThighPosition = landmarks[(int)poseIndex.RightHip].Position;
            pose.RightKneePosition = landmarks[(int)poseIndex.RightKnee].Position;
            pose.RightFootPosition = landmarks[(int)poseIndex.RightAnkle].Position;
        }
    }
}

