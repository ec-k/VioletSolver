// Copyright (c) 2024 HaruCoded
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System;
using System.Collections;
using System.Collections.Generic;

namespace VioletSolver
{
	public class CorutineUtils {
		public static IEnumerator HandleExceptions(IEnumerator enumerator, Action<Exception> errors) {
			var stack = new Stack<IEnumerator>();
			stack.Push(enumerator);

			while (stack.Count > 0) {
				var currentEnumerator = stack.Peek();
				object current;

				try {
					if (!currentEnumerator.MoveNext()) {
						stack.Pop();
						continue;
					}

					current = currentEnumerator.Current;
				} catch (Exception e) {
					errors(e);
					yield break;
				}

				if (current is IEnumerator yieldedEnumerator) {
					stack.Push(yieldedEnumerator);
				} else {
					yield return current;
				}
			}

			errors(null);
		}
	}
}
