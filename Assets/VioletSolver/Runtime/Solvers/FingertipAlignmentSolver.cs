using UnityEngine;
using VioletSolver.Landmarks;
using handIndex = HumanLandmarks.HandLandmarks.Types.LandmarkIndex;

namespace VioletSolver.Solver
{
    /// <summary>
    /// Calculates IK wrist position to align avatar's index finger distal with MediaPipe.Hand index finger DIP.
    /// </summary>
    internal static class FingertipAlignmentSolver
    {
        /// <summary>
        /// Calculates the adjusted IK wrist position.
        /// </summary>
        /// <param name="handLandmarks">MediaPipe.Hand landmarks</param>
        /// <param name="kinectWristPos">Wrist position from Kinect/Pose landmarks</param>
        /// <param name="avatarWristToDistal">Current avatar's wrist to index distal vector (from current pose)</param>
        /// <returns>Adjusted wrist position for IK target</returns>
        internal static Vector3 SolveWristPosition(
            in ILandmarkList handLandmarks,
            Vector3 kinectWristPos,
            Vector3 avatarWristToDistal)
        {
            // Get MediaPipe.Hand wrist and index finger DIP
            var mpHandWrist = handLandmarks.Landmarks[(int)handIndex.Wrist].Position;
            var mpHandIndexDip = handLandmarks.Landmarks[(int)handIndex.IndexFingerDip].Position;

            // User's wrist to DIP offset (in MediaPipe.Hand local space)
            var mpWristToDip = mpHandIndexDip - mpHandWrist;

            // Target DIP position = KinectPose.Wrist + (MediaPipe.Hand.DIP - MediaPipe.Hand.Wrist)
            var targetIndexDip = kinectWristPos + mpWristToDip;

            // Avatar wrist position = target DIP - avatar's current wrist-to-distal vector
            var ikWristPosition = targetIndexDip - avatarWristToDistal;

            return ikWristPosition;
        }
    }
}
