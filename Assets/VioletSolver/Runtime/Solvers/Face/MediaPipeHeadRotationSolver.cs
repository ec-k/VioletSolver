// Copyright (c) 2024 HaruCoded
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections.Generic;
using UnityEngine;
using VioletSolver.Landmarks;

namespace VioletSolver.Solver.Face
{
    internal static class MediaPipeHeadRotationSolver
    {
        /// <summary>
        /// Calculate head rotation from MediaPipe Face Mesh landmarks.
        /// </summary>
        /// <param name="landmarks">Face Mesh of MediaPipe.Face</param>
        /// <returns></returns>
        internal static Quaternion Solve(in IReadOnlyList<Landmark> landmarks)
        {
            Vector3 botHead = landmarks[152].Position;
            Vector3 topHead = landmarks[10].Position;
            Plane plane = new(landmarks[109].Position, landmarks[338].Position, botHead);

            Vector3 forwardDir = plane.normal;
            Vector3 faceUpDir = botHead - topHead;

            if (forwardDir == Vector3.zero || faceUpDir == Vector3.zero)
                return Quaternion.identity;

            return Quaternion.LookRotation(forwardDir, faceUpDir);
        }
    }
}

