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
using poseIndex = HumanLandmarks.KinectPoseLandmarks.Types.LandmarkIndex;


namespace VioletSolver.Solver
{
	internal class KinectPoseSolver
	{
		internal static AvatarPoseData SolvePose(in IReadOnlyList<Landmark> landmarks, in AvatarBones restBones, bool useIk, float scale = 1f)
		{
            AvatarPoseData pose = new();

			{
				Vector3 rShoulder = landmarks[(int)poseIndex.ShoulderRight].Position;
				Vector3 lShoulder = landmarks[(int)poseIndex.ShoulderLeft].Position;
				Vector3 rHip = landmarks[(int)poseIndex.HipRight].Position;
				Vector3 lHip = landmarks[(int)poseIndex.HipLeft].Position;

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
					) * scale;
				}

				if (useIk)
					SolveIkTargets(landmarks, ref pose, restBones, scale);
				else
				{
					// Arms
					{
						Vector4 rElbow = landmarks[(int)poseIndex.ElbowRight].Position;
						Vector4 rHand = landmarks[(int)poseIndex.WristRight].Position;
						var baseDirection = restBones.RightLowerArm.Position - restBones.RightUpperArm.Position;
						(pose.RightUpperArm, pose.RightLowerArm) = SolveRim(rShoulder, rElbow, rHand, baseDirection);
					}

					{
						Vector4 lElbow = landmarks[(int)poseIndex.ElbowLeft].Position;
						Vector4 lHand = landmarks[(int)poseIndex.WristLeft].Position;
						var baseDirection = restBones.LeftLowerArm.Position - restBones.LeftUpperArm.Position;
						(pose.LeftUpperArm, pose.LeftLowerArm) = SolveRim(lShoulder, lElbow, lHand, baseDirection);
					}
				}

				// Legs
				{
					Vector4 rKnee = landmarks[(int)poseIndex.KneeRight].Position;
					Vector4 rAnkle = landmarks[(int)poseIndex.AnkleRight].Position;
                    var baseDirection = restBones.RightLowerLeg.Position - restBones.RightUpperLeg.Position;
                    (pose.RightUpperLeg, pose.RightLowerLeg) = SolveRim(rHip, rKnee, rAnkle, baseDirection);
				}

				{
					Vector4 lKnee = landmarks[(int)poseIndex.KneeLeft].Position;
					Vector4 lAnkle = landmarks[(int)poseIndex.AnkleLeft].Position;
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

        static void SolveIkTargets(in IReadOnlyList<Landmark> landmarks, ref AvatarPoseData pose, in AvatarBones restBones, float scale)
        {
			var faceOffsetDirection = landmarks[(int)poseIndex.Nose].Position - landmarks[(int)poseIndex.Head].Position;

            // Positions
            pose.HeadPosition = (landmarks[(int)poseIndex.Head].Position + 0.2f * faceOffsetDirection) * scale;
            pose.HipsPosition = landmarks[(int)poseIndex.Pelvis].Position * scale;
			pose.ChestPosition = landmarks[(int)poseIndex.SpineChest].Position * scale;

            pose.LeftHandPosition = landmarks[(int)poseIndex.WristLeft].Position * scale;
            pose.LeftElbowPosition = landmarks[(int)poseIndex.ElbowLeft].Position * scale;
            pose.LeftShoulderPosition = landmarks[(int)poseIndex.ShoulderLeft].Position * scale;

            pose.RightHandPosition = landmarks[(int)poseIndex.WristRight].Position * scale;
            pose.RightElbowPosition = landmarks[(int)poseIndex.ElbowRight].Position * scale;
            pose.RightShoulderPosition = landmarks[(int)poseIndex.ShoulderRight].Position * scale;

			pose.LeftThighPosition = landmarks[(int)poseIndex.HipLeft].Position * scale;
			pose.LeftKneePosition = landmarks[(int)poseIndex.KneeLeft].Position * scale;
			pose.LeftFootPosition = landmarks[(int)poseIndex.FootLeft].Position * scale;

            pose.RightThighPosition = landmarks[(int)poseIndex.HipRight].Position * scale;
            pose.RightKneePosition = landmarks[(int)poseIndex.KneeRight].Position * scale;
            pose.RightFootPosition = landmarks[(int)poseIndex.FootRight].Position * scale;

            // Rotations
			if(landmarks[(int)poseIndex.Head].Rotation.HasValue)
				pose.Head = landmarks[(int)poseIndex.Head].Rotation.Value * restBones.Head.Rotation;
        }
    }
}
