// Copyright (c) 2024 HaruCoded
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using UnityEngine;
using handIndex = HumanLandmarks.HandLandmarks.Types.LandmarkIndex;

namespace VioletSolver
{
	public class HandData
	{
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
		public Quaternion this[int index]
		{
			get
			{
				return index switch
				{
					(int)handIndex.Wrist => Wrist,
					(int)handIndex.ThumbCmc => ThumbCMC,
					(int)handIndex.ThumbMcp => ThumbMCP,
					(int)handIndex.ThumbIp => ThumbIP,
					(int)handIndex.ThumbTip => ThumbTIP,
					(int)handIndex.IndexFingerMcp => IndexFingerMCP,
					(int)handIndex.IndexFingerPip => IndexFingerPIP,
					(int)handIndex.IndexFingerDip => IndexFingerDIP,
					(int)handIndex.IndexFingerTip => IndexFingerTIP,
					(int)handIndex.MiddleFingerMcp => MiddleFingerMCP,
					(int)handIndex.MiddleFingerPip => MiddleFingerPIP,
					(int)handIndex.MiddleFingerDip => MiddleFingerDIP,
					(int)handIndex.MiddleFingerTip => MiddleFingerTIP,
					(int)handIndex.RingFingerMcp => RingFingerMCP,
					(int)handIndex.RingFingerPip => RingFingerPIP,
					(int)handIndex.RingFingerDip => RingFingerDIP,
					(int)handIndex.RingFingerTip => RingFingerTIP,
					(int)handIndex.PinkyMcp => PinkyMCP,
					(int)handIndex.PinkyPip => PinkyPIP,
					(int)handIndex.PinkyDip => PinkyDIP,
					(int)handIndex.PinkyTip => PinkyTIP,
					_ => Quaternion.identity,
				};
			}
			set
			{
				switch (index)
				{
					case (int)handIndex.Wrist: Wrist = value; return;
					case (int)handIndex.ThumbCmc: ThumbCMC = value; return;
					case (int)handIndex.ThumbMcp: ThumbMCP = value; return;
					case (int)handIndex.ThumbIp: ThumbIP = value; return;
					case (int)handIndex.ThumbTip: ThumbTIP = value; return;
					case (int)handIndex.IndexFingerMcp: IndexFingerMCP = value; return;
					case (int)handIndex.IndexFingerPip: IndexFingerPIP = value; return;
					case (int)handIndex.IndexFingerDip: IndexFingerDIP = value; return;
					case (int)handIndex.IndexFingerTip: IndexFingerTIP = value; return;
					case (int)handIndex.MiddleFingerMcp: MiddleFingerMCP = value; return;
					case (int)handIndex.MiddleFingerPip: MiddleFingerPIP = value; return;
					case (int)handIndex.MiddleFingerDip: MiddleFingerDIP = value; return;
					case (int)handIndex.MiddleFingerTip: MiddleFingerTIP = value; return;
					case (int)handIndex.RingFingerMcp: RingFingerMCP = value; return;
					case (int)handIndex.RingFingerPip: RingFingerPIP = value; return;
					case (int)handIndex.RingFingerDip: RingFingerDIP = value; return;
					case (int)handIndex.RingFingerTip: RingFingerTIP = value; return;
					case (int)handIndex.PinkyMcp: PinkyMCP = value; return;
					case (int)handIndex.PinkyPip: PinkyPIP = value; return;
					case (int)handIndex.PinkyDip: PinkyDIP = value; return;
					case (int)handIndex.PinkyTip: PinkyTIP = value; return;
					default:
						{
							UnityEngine.Debug.LogError("Invalid index " + index);
							return;
						}
				}
			}
		}
	}
}
