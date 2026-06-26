using System.Collections.Generic;
using UnityEngine;
using VioletSolver.Landmarks;
using kinectIndex = HumanLandmarks.KinectPoseLandmarks.Types.LandmarkIndex;
using handIndex = HumanLandmarks.HandLandmarks.Types.LandmarkIndex;
using System.Linq;

namespace VioletSolver.Setup
{
    /// <summary>
    /// Calibrates the scale ratio between Kinect and MediaPipe.Hand skeleton by comparing hand lengths.
    /// </summary>
    public class HandScaleCalibrator
    {
        public bool IsCalibrated { get; private set; }
        public float Scale { get; private set; } = 1f;

        readonly List<float> _samples = new();
        readonly int _requiredSamples;
        readonly float _outlierThreshold;
        readonly float _confidenceThreshold;

        /// <summary>
        /// Returns the current calibration progress (0 to 1).
        /// </summary>
        public float Progress => Mathf.Clamp01((float)_samples.Count / _requiredSamples);

        /// <summary>
        /// Creates a new HandScaleCalibrator.
        /// </summary>
        /// <param name="requiredSamples">Number of samples required for calibration.</param>
        /// <param name="outlierThreshold">Threshold for rejecting outlier samples (ratio to median).</param>
        /// <param name="confidenceThreshold">Minimum landmark confidence to accept a sample.</param>
        public HandScaleCalibrator(int requiredSamples = 30, float outlierThreshold = 0.2f, float confidenceThreshold = 0.5f)
        {
            _requiredSamples = requiredSamples;
            _outlierThreshold = outlierThreshold;
            _confidenceThreshold = confidenceThreshold;
        }

        /// <summary>
        /// Updates the calibrator with new landmark data.
        /// </summary>
        /// <param name="kinectPoseLandmarks">Kinect pose landmarks.</param>
        /// <param name="leftHandLandmarks">MediaPipe left hand landmarks.</param>
        /// <param name="rightHandLandmarks">MediaPipe right hand landmarks.</param>
        public void Update(ILandmarkList kinectPoseLandmarks, ILandmarkList leftHandLandmarks, ILandmarkList rightHandLandmarks)
        {
            if (IsCalibrated) return;
            if (kinectPoseLandmarks == null || leftHandLandmarks == null || rightHandLandmarks == null) return;

            var frameScale = CalculateFrameScale(kinectPoseLandmarks, leftHandLandmarks, rightHandLandmarks);
            if (frameScale <= 0f) return;

            _samples.Add(frameScale);

            if (_samples.Count >= _requiredSamples)
                Calibrate();
        }

        /// <summary>
        /// Resets the calibrator to start a new calibration.
        /// </summary>
        public void Reset()
        {
            IsCalibrated = false;
            Scale = 1f;
            _samples.Clear();
        }

        void Calibrate()
        {
            var filteredSamples = FilterOutliers(_samples);
            if (filteredSamples.Count == 0)
            {
                // All samples were outliers, retry.
                _samples.Clear();
                return;
            }

            Scale = filteredSamples.Average();
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

        float CalculateFrameScale(ILandmarkList kinectPoseLandmarks, ILandmarkList leftHandLandmarks, ILandmarkList rightHandLandmarks)
        {
            var leftScale = CalculateSingleHandScale(
                kinectPoseLandmarks,
                kinectIndex.WristLeft,
                kinectIndex.HandtipLeft,
                leftHandLandmarks);

            var rightScale = CalculateSingleHandScale(
                kinectPoseLandmarks,
                kinectIndex.WristRight,
                kinectIndex.HandtipRight,
                rightHandLandmarks);

            // Skip frame if either hand is invalid.
            if (leftScale <= 0f || rightScale <= 0f) return 0f;

            return (leftScale + rightScale) / 2f;
        }

        float CalculateSingleHandScale(ILandmarkList kinectPoseLandmarks, kinectIndex wristIndex, kinectIndex handtipIndex, ILandmarkList mpHandLandmarks)
        {
            var kinectWrist = kinectPoseLandmarks.Landmarks[(int)wristIndex];
            var kinectHandtip = kinectPoseLandmarks.Landmarks[(int)handtipIndex];

            if (kinectWrist.Confidence < _confidenceThreshold) return 0f;
            if (kinectHandtip.Confidence < _confidenceThreshold) return 0f;

            var kinectHandLength = (kinectHandtip.Position - kinectWrist.Position).magnitude;
            if (kinectHandLength <= 0f) return 0f;

            var mpWrist = mpHandLandmarks.Landmarks[(int)handIndex.Wrist];
            var mpMiddleTip = mpHandLandmarks.Landmarks[(int)handIndex.MiddleFingerTip];

            if (mpWrist.Confidence < _confidenceThreshold) return 0f;
            if (mpMiddleTip.Confidence < _confidenceThreshold) return 0f;

            var mpHandLength = (mpMiddleTip.Position - mpWrist.Position).magnitude;
            if (mpHandLength <= 0f) return 0f;

            return kinectHandLength / mpHandLength;
        }
    }
}
