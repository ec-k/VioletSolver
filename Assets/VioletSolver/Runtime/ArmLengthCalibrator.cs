using System.Collections.Generic;
using UnityEngine;
using VioletSolver.Landmarks;
using kinectIndex = HumanLandmarks.KinectPoseLandmarks.Types.LandmarkIndex;

namespace VioletSolver
{
    /// <summary>
    /// Calibrates the scale ratio between user and avatar based on arm length.
    /// Measures the distance from wrist to shoulder (upper arm + lower arm).
    /// </summary>
    public class ArmLengthCalibrator
    {
        public bool IsCalibrated { get; private set; }
        public float Scale { get; private set; } = 1f;

        readonly float _avatarArmLength;
        readonly List<float> _userArmLengthSamples = new();
        readonly int _requiredSamples;
        readonly float _outlierThreshold;

        /// <summary>
        /// Creates a new ArmLengthCalibrator.
        /// </summary>
        /// <param name="animator">The avatar's Animator component.</param>
        /// <param name="requiredSamples">Number of samples required for calibration.</param>
        /// <param name="outlierThreshold">Threshold for rejecting outlier samples (ratio to median).</param>
        public ArmLengthCalibrator(Animator animator, int requiredSamples = 30, float outlierThreshold = 0.2f)
        {
            _requiredSamples = requiredSamples;
            _outlierThreshold = outlierThreshold;
            _avatarArmLength = CalculateAvatarArmLength(animator);
        }

        /// <summary>
        /// Updates the calibrator with new landmark data.
        /// </summary>
        /// <param name="landmarks">Kinect pose landmarks.</param>
        public void Update(ILandmarkList landmarks)
        {
            if (IsCalibrated) return;
            if (landmarks == null) return;

            var userArmLength = CalculateUserArmLength(landmarks);
            if (userArmLength <= 0f) return;

            _userArmLengthSamples.Add(userArmLength);

            if (_userArmLengthSamples.Count >= _requiredSamples)
                Calibrate();
        }

        /// <summary>
        /// Resets the calibrator to start a new calibration.
        /// </summary>
        public void Reset()
        {
            IsCalibrated = false;
            Scale = 1f;
            _userArmLengthSamples.Clear();
        }

        /// <summary>
        /// Returns the current calibration progress (0 to 1).
        /// </summary>
        public float Progress => Mathf.Clamp01((float)_userArmLengthSamples.Count / _requiredSamples);

        void Calibrate()
        {
            var filteredSamples = FilterOutliers(_userArmLengthSamples);
            if (filteredSamples.Count == 0)
            {
                // All samples were outliers, retry.
                _userArmLengthSamples.Clear();
                return;
            }

            var averageUserArmLength = 0f;
            foreach (var sample in filteredSamples)
                averageUserArmLength += sample;
            averageUserArmLength /= filteredSamples.Count;

            Scale = _avatarArmLength / averageUserArmLength;
            IsCalibrated = true;
        }

        List<float> FilterOutliers(List<float> samples)
        {
            if (samples.Count == 0) return samples;

            // Calculate median.
            var sorted = new List<float>(samples);
            sorted.Sort();
            var median = sorted[sorted.Count / 2];

            // Filter samples within threshold of median.
            var filtered = new List<float>();
            foreach (var sample in samples)
            {
                var ratio = Mathf.Abs(sample - median) / median;
                if (ratio <= _outlierThreshold)
                    filtered.Add(sample);
            }

            return filtered;
        }

        float CalculateAvatarArmLength(Animator animator)
        {
            // Calculate average of both arms.
            var leftLength = CalculateAvatarSingleArmLength(animator,
                HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand);
            var rightLength = CalculateAvatarSingleArmLength(animator,
                HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand);

            return (leftLength + rightLength) / 2f;
        }

        float CalculateAvatarSingleArmLength(Animator animator,
            HumanBodyBones shoulder, HumanBodyBones elbow, HumanBodyBones wrist)
        {
            var shoulderPos = animator.GetBoneTransform(shoulder).position;
            var elbowPos = animator.GetBoneTransform(elbow).position;
            var wristPos = animator.GetBoneTransform(wrist).position;

            return (elbowPos - shoulderPos).magnitude + (wristPos - elbowPos).magnitude;
        }

        float CalculateUserArmLength(ILandmarkList landmarks)
        {
            var leftLength = CalculateUserSingleArmLength(landmarks,
                kinectIndex.ShoulderLeft, kinectIndex.ElbowLeft, kinectIndex.WristLeft);
            var rightLength = CalculateUserSingleArmLength(landmarks,
                kinectIndex.ShoulderRight, kinectIndex.ElbowRight, kinectIndex.WristRight);

            // Use average of both arms, but skip if either is invalid.
            if (leftLength <= 0f || rightLength <= 0f)
                return 0f;

            return (leftLength + rightLength) / 2f;
        }

        float CalculateUserSingleArmLength(ILandmarkList landmarks,
            kinectIndex shoulder, kinectIndex elbow, kinectIndex wrist)
        {
            var shoulderLandmark = landmarks.Landmarks[(int)shoulder];
            var elbowLandmark = landmarks.Landmarks[(int)elbow];
            var wristLandmark = landmarks.Landmarks[(int)wrist];

            // Check confidence if available.
            if (shoulderLandmark.Confidence < 0.5f ||
                elbowLandmark.Confidence < 0.5f ||
                wristLandmark.Confidence < 0.5f)
                return 0f;

            var upperArm = (elbowLandmark.Position - shoulderLandmark.Position).magnitude;
            var lowerArm = (wristLandmark.Position - elbowLandmark.Position).magnitude;

            return upperArm + lowerArm;
        }
    }
}
