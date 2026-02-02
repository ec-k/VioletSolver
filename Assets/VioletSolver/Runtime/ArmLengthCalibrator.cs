using System.Collections.Generic;
using System.Linq;
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

        readonly HumanBodyBones[] _avatarLeftArmBones = new HumanBodyBones[]
        {
            HumanBodyBones.LeftUpperArm,
            HumanBodyBones.LeftLowerArm,
            HumanBodyBones.LeftHand,
            //HumanBodyBones.LeftMiddleDistal
        };
        readonly HumanBodyBones[] _avatarRightArmBones = new HumanBodyBones[]
        {
            HumanBodyBones.RightUpperArm,
            HumanBodyBones.RightLowerArm,
            HumanBodyBones.RightHand,
            //HumanBodyBones.RightMiddleDistal
        };
        readonly kinectIndex[] _userLeftArmBones = new kinectIndex[]
        {
            kinectIndex.ShoulderLeft,
            kinectIndex.ElbowLeft,
            kinectIndex.WristLeft,
            //kinectIndex.HandtipLeft
        };
        readonly kinectIndex[] _userRightArmBones = new kinectIndex[]
        {
            kinectIndex.ShoulderRight,
            kinectIndex.ElbowRight,
            kinectIndex.WristRight,
            //kinectIndex.HandtipRight
        };

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

        List<float> FilterOutliers(IReadOnlyList<float> samples)
        {
            if (samples.Count == 0) return new();

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
            var leftLength = CalculateAvatarSingleArmLength(animator, _avatarLeftArmBones);
            var rightLength = CalculateAvatarSingleArmLength(animator, _avatarRightArmBones);

            return (leftLength + rightLength) / 2f;
        }

        float CalculateAvatarSingleArmLength(Animator animator, HumanBodyBones[] boneNames)
            //HumanBodyBones shoulder, HumanBodyBones elbow, HumanBodyBones wrist, HumanBodyBones handTip)
        {
            var length = 0f;

            for(var i=0;i<boneNames.Length - 1;i++)
            {
                var startPos = animator.GetBoneTransform(boneNames[i]).position;
                var endPos = animator.GetBoneTransform(boneNames[i + 1]).position;
                length += (endPos - startPos).magnitude;
            }

            return length;
        }

        float CalculateUserArmLength(ILandmarkList landmarks)
        {
            var leftLength = CalculateUserSingleArmLength(landmarks, _userLeftArmBones);
            var rightLength = CalculateUserSingleArmLength(landmarks, _userRightArmBones);

            // Use average of both arms, but skip if either is invalid.
            if (leftLength <= 0f || rightLength <= 0f)
                return 0f;

            return (leftLength + rightLength) / 2f;
        }

        float CalculateUserSingleArmLength(ILandmarkList landmarks, kinectIndex[] boneNames)
        {
            var length = 0f;

            for(var i=0;i<boneNames.Length - 1;i++)
            {
                var startLandmark = landmarks.Landmarks[(int)boneNames[i]];
                var endLandmark = landmarks.Landmarks[(int)boneNames[i + 1]];
                // Check confidence if available.
                if (startLandmark.Confidence < 0.5f ||
                    endLandmark.Confidence < 0.5f)
                    return 0f;
                length += (endLandmark.Position - startLandmark.Position).magnitude;
            }

            return length;
        }
    }
}
