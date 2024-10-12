// Copyright (c) 2024 HaruCoded
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using UnityEngine;

namespace VioletSolver
{
	public class Settings {
		private const string _TAG = nameof(Settings);

		// This must be called to initialize the system
		public static void Init() {
			foreach (IField field in IField.DefinedSettings) {
				field.Init();
			}
		}

		public static class Temporary
		{
			public static bool VirtualCamera = false;
		}

		// Bone Settings
		public static Bool _UseLegRotation = new("bone.use.legs", false);
		public static Float _HandTrackingThreshold = new("tracking.threshold.hand", 0f);
		public static Float _TrackingInterpolation = new("tracking.interpolation", 0.1f);

		public static float TrackingInterpolation {
			get => _TrackingInterpolation.Get();
			set => _TrackingInterpolation.Set(value);
		}

		// Gui Settings
		public static Text _ModelFile = new("gui.model", "");

		public static string ModelFile {
			get => _ModelFile.Get();
			set => _ModelFile.Set(value);
		}

		public delegate void IntDelegate (int value);

		// Reset
		public static void ResetSettings() {
			PlayerPrefs.DeleteAll();
		}
	}
}
