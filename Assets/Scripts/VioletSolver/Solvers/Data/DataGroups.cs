// Copyright (c) 2024 HaruCoded
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using UnityEngine;
using HolisticPose;

namespace VioletSolver
{
	public class DataGroups {
		public class HandPoints {
			// This uses the indexs MediaPipe.Hand
			public readonly Vector3[] Data = new Vector3[21];
			
			// Helper properties
			public Vector3 Wrist { get => Data[0]; set => Data[0] = value; }
			public Vector3 ThumbCMC { get => Data[1]; set => Data[1] = value; }
			public Vector3 ThumbMCP { get => Data[2]; set => Data[2] = value; }
			public Vector3 ThumbIP { get => Data[3]; set => Data[3] = value; }
			public Vector3 ThumbTIP { get => Data[4]; set => Data[4] = value; }
			public Vector3 IndexFingerMCP { get => Data[5]; set => Data[5] = value; }
			public Vector3 IndexFingerPIP { get => Data[6]; set => Data[6] = value; }
			public Vector3 IndexFingerDIP { get => Data[7]; set => Data[7] = value; }
			public Vector3 IndexFingerTIP { get => Data[8]; set => Data[8] = value; }
			public Vector3 MiddleFingerMCP { get => Data[9]; set => Data[9] = value; }
			public Vector3 MiddleFingerPIP { get => Data[10]; set => Data[10] = value; }
			public Vector3 MiddleFingerDIP { get => Data[11]; set => Data[11] = value; }
			public Vector3 MiddleFingerTIP { get => Data[12]; set => Data[12] = value; }
			public Vector3 RingFingerMCP { get => Data[13]; set => Data[13] = value; }
			public Vector3 RingFingerPIP { get => Data[14]; set => Data[14] = value; }
			public Vector3 RingFingerDIP { get => Data[15]; set => Data[15] = value; }
			public Vector3 RingFingerTIP { get => Data[16]; set => Data[16] = value; }
			public Vector3 PinkyMCP { get => Data[17]; set => Data[17] = value; }
			public Vector3 PinkyPIP { get => Data[18]; set => Data[18] = value; }
			public Vector3 PinkyDIP { get => Data[19]; set => Data[19] = value; }
			public Vector3 PinkyTIP { get => Data[20]; set => Data[20] = value; }
		}

		public class HandData {
			public readonly int Length = 21;

			// Easy setters
			public Quaternion Wrist;
			public Quaternion ThumbCMC;
			public Quaternion ThumbMCP;
			public Quaternion ThumbIP;
			public Quaternion ThumbTIP;
			public Quaternion IndexFingerMCP;
			public Quaternion IndexFingerPIP;
			public Quaternion IndexFingerDIP;
			public Quaternion IndexFingerTIP;
			public Quaternion MiddleFingerMCP;
			public Quaternion MiddleFingerPIP;
			public Quaternion MiddleFingerDIP;
			public Quaternion MiddleFingerTIP;
			public Quaternion RingFingerMCP;
			public Quaternion RingFingerPIP;
			public Quaternion RingFingerDIP;
			public Quaternion RingFingerTIP;
			public Quaternion PinkyMCP;
			public Quaternion PinkyPIP;
			public Quaternion PinkyDIP;
			public Quaternion PinkyTIP;
			
			// This uses the indexs MediaPipe.Hand
			public Quaternion this[int index] {
				get {
					return index switch {
						(int)HandLandmark.Wrist => Wrist,
						(int)HandLandmark.ThumbCmc => ThumbCMC,
						(int)HandLandmark.ThumbMcp => ThumbMCP,
						(int)HandLandmark.ThumbIp => ThumbIP,
						(int)HandLandmark.ThumbTip => ThumbTIP,
						(int)HandLandmark.IndexFingerMcp => IndexFingerMCP,
						(int)HandLandmark.IndexFingerPip=> IndexFingerPIP,
						(int)HandLandmark.IndexFingerDip => IndexFingerDIP,
						(int)HandLandmark.IndexFingerTip => IndexFingerTIP,
						(int)HandLandmark.MiddleFingerMcp => MiddleFingerMCP,
						(int)HandLandmark.MiddleFingerPip => MiddleFingerPIP,
						(int)HandLandmark.MiddleFingerDip => MiddleFingerDIP,
						(int)HandLandmark.MiddleFingerTip => MiddleFingerTIP,
						(int)HandLandmark.RingFingerMcp => RingFingerMCP,
						(int)HandLandmark.RingFingerPip => RingFingerPIP,
						(int)HandLandmark.RingFingerDip => RingFingerDIP,
						(int)HandLandmark.RingFingerTip => RingFingerTIP,
						(int)HandLandmark.PinkyMcp => PinkyMCP,
						(int)HandLandmark.PinkyPip => PinkyPIP,
						(int)HandLandmark.PinkyDip => PinkyDIP,
						(int)HandLandmark.PinkyTip => PinkyTIP,
						_ => Quaternion.identity,
					};
				}
				set {
					switch (index) {
						case (int)HandLandmark.Wrist : Wrist = value; return;
						case (int)HandLandmark.ThumbCmc : ThumbCMC = value; return;
						case (int)HandLandmark.ThumbMcp : ThumbMCP = value; return;
						case (int)HandLandmark.ThumbIp : ThumbIP = value; return;
						case (int)HandLandmark.ThumbTip : ThumbTIP = value; return;
						case (int)HandLandmark.IndexFingerMcp : IndexFingerMCP = value; return;
						case (int)HandLandmark.IndexFingerPip: IndexFingerPIP = value; return;
						case (int)HandLandmark.IndexFingerDip : IndexFingerDIP = value; return;
						case (int)HandLandmark.IndexFingerTip : IndexFingerTIP = value; return;
						case (int)HandLandmark.MiddleFingerMcp : MiddleFingerMCP = value; return;
						case (int)HandLandmark.MiddleFingerPip : MiddleFingerPIP = value; return;
						case (int)HandLandmark.MiddleFingerDip : MiddleFingerDIP = value; return;
						case (int)HandLandmark.MiddleFingerTip : MiddleFingerTIP = value; return;
						case (int)HandLandmark.RingFingerMcp : RingFingerMCP = value; return;
						case (int)HandLandmark.RingFingerPip : RingFingerPIP = value; return;
						case (int)HandLandmark.RingFingerDip : RingFingerDIP = value; return;
						case (int)HandLandmark.RingFingerTip : RingFingerTIP = value; return;
						case (int)HandLandmark.PinkyMcp : PinkyMCP = value; return;
						case (int)HandLandmark.PinkyPip : PinkyPIP = value; return;
						case (int)HandLandmark.PinkyDip : PinkyDIP = value; return;
						case (int)HandLandmark.PinkyTip : PinkyTIP = value; return;
						default: {
							Debug.LogError("Invalid index " + index);
							return;
						}
					}
				}
			}
		}
		
		public class PoseData {
			public Quaternion chestRotation;
			public Quaternion hipsRotation;

			public Quaternion neckRotation;

			public Vector3 hipsPosition;

			public Quaternion rUpperLeg;
			public Quaternion rLowerLeg;
			public Quaternion lUpperLeg;
			public Quaternion lLowerLeg;

			// IK Positions
			public Vector4 rShoulder;
			public Vector4 rElbow;
			public Vector4 rHand;
			public Vector4 lShoulder;
			public Vector4 lElbow;
			public Vector4 lHand;

			public bool hasLeftLeg;
			public bool hasRightLeg;
		}

		public class FaceData {
			// Mouth
			public float mouthOpen;

			// Eyes
			public Vector2 lEyeIris;
			public Vector2 rEyeIris;
			public float lEyeOpen;
			public float rEyeOpen;

			// Neck
			public Quaternion neckRotation;
		}
	}
}
