using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VioletSolver.Landmarks
{
    /// <summary>
    /// Interface that holds pose, hand, and face landmarks.
    /// </summary>
    public interface IHolisticLandmarks
    {
        public ILandmarkList Pose { get; }
        public ILandmarkList LeftHand { get; }
        public ILandmarkList RightHand { get; }
        public ILandmarkList Face { get; }

        /// <summary>
        /// Updates the landmark data.
        /// </summary>
        /// <param name="landmarks">The received landmark data.</param>
        /// <param name="time">The time the data was received.</param>
        public void UpdateLandmarks(HumanLandmarks.HolisticLandmarks landmarks, float time);
    }

    /// <summary>
    /// Interface that holds a list of landmarks.
    /// </summary>
    public interface ILandmarkList
    {
        public List<Landmark> Landmarks { get; set;  }
        public int Count { get; }
        public float Time { get; }
    }

    /// <summary>
    /// Holds and updates a list of landmarks.
    /// </summary>
    public class LandmarkList : ILandmarkList
    {
        public List<Landmark> Landmarks { get; set; }
        public int Count => Landmarks.Count;
        public float Time { get; private set; }

        internal LandmarkList(int size)
        {
            Landmarks = new Landmark[size].ToList();
            Time = 0f;
        }

        /// <summary>
        /// Updates the list with Landmark-type landmark data.
        /// </summary>
        /// <param name="landmarks">The landmark data.</param>
        /// <param name="time">The time the data was received.</param>
        public void Set(IReadOnlyList<HumanLandmarks.Landmark> landmarks, float time)
        {
            for (var i = 0; i < Count; i++)
                Landmarks[i] = new(landmarks[i]);
            Time = time;
        }
        /// <summary>
        /// Updates the list with LandmarkPoint-type landmark data.
        /// </summary>
        /// <param name="landmarks">The landmark point data.</param>
        /// <param name="time">The time the data was received.</param>
        public void Set(IReadOnlyList<HumanLandmarks.LandmarkPoint> landmarks, float time)
        {
            for (var i = 0; i < Count; i++)
                Landmarks[i] = new(landmarks[i]);
            Time = time;
        }
    }

    /// <summary>
    /// Landmark data with position, rotation, and confidence.
    /// </summary>
    public struct Landmark
    {
        public Vector3 Position { get; private set; }
        public Quaternion? Rotation { get; private set; }
        public float Confidence { get; set; }

        public Landmark(HumanLandmarks.Landmark landmark)
        {
            Position = new Vector3(
                landmark.Position.X,
                landmark.Position.Y,
                landmark.Position.Z);
            Rotation = new Quaternion(
                landmark.Rotation.X,
                landmark.Rotation.Y,
                landmark.Rotation.Z,
                landmark.Rotation.W);
            Confidence = landmark.Confidence;
        }
        public Landmark(HumanLandmarks.LandmarkPoint landmark)
        {
            Position = new Vector3(
                landmark.Position.X,
                landmark.Position.Y,
                landmark.Position.Z);
            Rotation = null;
            Confidence = landmark.Confidence;
        }

        public Landmark(Vector3 position, Quaternion rotation, float confidence)
        {
            Position = position;
            Rotation = rotation;
            Confidence = confidence;
        }
        public Landmark(Vector3 position, float confidence)
        {
            Position = position;
            Rotation = null;
            Confidence = confidence;
        }
        public Landmark(float x, float y, float z, float confidence = 0f)
        {
            Position = new Vector3(x, y, z);
            Rotation = null;
            Confidence = confidence;
        }

        /// <summary>
        /// Linearly interpolates between two landmarks.
        /// </summary>
        /// <param name="l1">The start landmark.</param>
        /// <param name="l2">The end landmark.</param>
        /// <param name="amount">The interpolation factor (0 to 1).</param>
        /// <returns>The interpolated landmark.</returns>
        public static Landmark Lerp(Landmark l1, Landmark l2, float amount)
        {
            Quaternion? rotation = l1.Rotation.HasValue && l2.Rotation.HasValue
                ? Quaternion.Slerp(l1.Rotation.Value, l2.Rotation.Value, amount)
                : null;

            return new()
            {
                Position   = Vector3.Lerp(l1.Position, l2.Position, amount),
                Rotation   = rotation,
                Confidence = Mathf.Lerp(l1.Confidence, l2.Confidence, amount)
            };
        }
    }
}
