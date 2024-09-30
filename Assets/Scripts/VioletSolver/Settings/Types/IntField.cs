// Copyright (c) 2024 HaruCoded
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System;
using UnityEngine;

namespace VioletSolver
{
	public class SafeInt : Int {
		public SafeInt(string key, int def, Action<int> callback = null) : base(key, def, callback) {}
		public override object RawValue() {
			return "*";
		}
	}

	public class Int : Field<int> {
		private int m_value;
		public Int(string key, int def, Action<int> callback = null) : base(key, def, callback) {}
			
		public override void Reset() {
			Set(m_def);
		}

		public override void Init() {
			m_value = PlayerPrefs.GetInt(m_key, m_def);
		}

		public override int Get() {
			return m_value;
		}

		public override void Set(int value) {
			m_value = value;
			PlayerPrefs.SetInt(m_key, value);
			m_callback?.Invoke(value);
		}
	}
}
