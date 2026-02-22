using UnityEngine;

namespace VioletSolver.Pose
{
    /// <summary>
    /// One Euro Filter for hand IK positions.
    /// Adapts smoothing based on velocity - smooth when slow, responsive when fast.
    /// </summary>
    public class HandPositionOneEuroFilter : IAvatarPoseFilter
    {
        readonly PositionOneEuroFilter _leftHandFilter;
        readonly PositionOneEuroFilter _rightHandFilter;

        /// <summary>
        /// Creates a new hand position One Euro filter.
        /// </summary>
        /// <param name="minCutoff">Minimum cutoff frequency. Lower = smoother when slow.</param>
        /// <param name="beta">Slope coefficient. Higher = less lag when fast.</param>
        /// <param name="dCutoff">Cutoff frequency for velocity smoothing.</param>
        public HandPositionOneEuroFilter(float minCutoff = 1.0f, float beta = 0.007f, float dCutoff = 1.0f)
        {
            _leftHandFilter = new PositionOneEuroFilter(minCutoff, beta, dCutoff);
            _rightHandFilter = new PositionOneEuroFilter(minCutoff, beta, dCutoff);
        }

        public AvatarPoseData Filter(AvatarPoseData pose)
        {
            float timestamp = pose.time;

            pose.LeftHandPosition = _leftHandFilter.Apply(pose.LeftHandPosition, timestamp);
            pose.RightHandPosition = _rightHandFilter.Apply(pose.RightHandPosition, timestamp);

            return pose;
        }
    }

    /// <summary>
    /// One Euro Filter implementation for Vector3 (position) smoothing.
    /// </summary>
    internal class PositionOneEuroFilter
    {
        readonly float _minCutoff;
        readonly float _beta;
        readonly float _dCutoff;

        float _prevTime;
        bool _isFirst = true;

        readonly SingleExponentialFilter _positionFilter = new();
        readonly SingleExponentialFilter _velocityFilter = new();

        internal PositionOneEuroFilter(float minCutoff, float beta, float dCutoff)
        {
            _minCutoff = minCutoff;
            _beta = beta;
            _dCutoff = dCutoff;
        }

        internal Vector3 Apply(Vector3 current, float timestamp)
        {
            var dt = timestamp - _prevTime;
            if (dt <= 1e-5f) return _positionFilter.PrevResult ?? current;

            var updateRate = 1f / dt;
            var velocity = Vector3.zero;

            if (_isFirst || !_positionFilter.PrevResult.HasValue)
            {
                _prevTime = timestamp;
                _isFirst = false;
                _positionFilter.Apply(current, 1f); // Initialize
                return current;
            }

            // Calculate velocity
            velocity = (current - _positionFilter.PrevResult.Value) * updateRate;

            // Smooth the velocity
            var smoothedVelocity = _velocityFilter.Apply(velocity, Alpha(updateRate, _dCutoff));

            // Calculate adaptive cutoff based on velocity magnitude
            var cutoff = _minCutoff + _beta * smoothedVelocity.magnitude;

            // Apply smoothing to position
            var result = _positionFilter.Apply(current, Alpha(updateRate, cutoff));

            _prevTime = timestamp;
            return result;
        }

        float Alpha(float updateRate, float cutoffFrequency)
        {
            var timeConstant = 1f / (2f * Mathf.PI * cutoffFrequency);
            return 1f / (1f + timeConstant * updateRate);
        }
    }

    /// <summary>
    /// Single exponential smoothing filter for Vector3.
    /// </summary>
    internal class SingleExponentialFilter
    {
        internal Vector3? PrevResult { get; private set; }

        internal Vector3 Apply(Vector3 current, float alpha)
        {
            if (!PrevResult.HasValue)
            {
                PrevResult = current;
                return current;
            }

            var result = Vector3.Lerp(PrevResult.Value, current, alpha);
            PrevResult = result;
            return result;
        }
    }
}
