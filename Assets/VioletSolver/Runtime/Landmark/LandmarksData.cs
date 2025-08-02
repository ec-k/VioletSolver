using Google.Protobuf.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VioletSolver.Landmarks
{
    public interface IHolisticLandmarks
    {
        public ILandmarks Pose { get; set; }
        public ILandmarks LeftHand { get; set; }
        public ILandmarks RightHand { get; set; }
        public ILandmarks Face { get; set; }

        public void UpdateLandmarks(HumanLandmarks.HolisticLandmarks landmarks, float time);
    }

    public interface ILandmarks
    {
        public List<Landmark> Landmarks { get; }
        public int Count => Landmarks.Count;
        public float Time { get; }
        public void UpdateLandmarks(RepeatedField<HumanLandmarks.Landmark> landmarks, float time);
    }

    public struct Landmark
    {
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }
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
        public Landmark(Vector3 position, Quaternion rotation, float confidence)
        {
            Position = position;
            Rotation = rotation;
            Confidence = confidence;
        }
        public Landmark(Vector3 position, float confidence)
        {
            Position = position;
            Rotation = Quaternion.identity;
            Confidence = confidence;
        }
        public Landmark(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
            Rotation = Quaternion.identity;
            Confidence = 0.0f;
        }
        public Landmark(float x, float y, float z, float confidence)
        {
            Position = new Vector3(x, y, z);
            Rotation = Quaternion.identity;
            Confidence = confidence;
        }

        public static Landmark SetFromHolisticLandmark(HumanLandmarks.Landmark landmark)
        {
            return new Landmark(
                landmark.Position.X,
                landmark.Position.Y,
                landmark.Position.Z,
                landmark.Confidence);
        }

        public static Landmark Lerp(Landmark l1, Landmark l2, float amount)
        => new Landmark(
                Vector3.Lerp(l1.Position, l2.Position, amount),
                Quaternion.Slerp(l1.Rotation, l2.Rotation, amount),
                Mathf.Lerp(l1.Confidence, l2.Confidence, amount));

        //public static implicit operator Vector3 (Landmark l) => new Vector3(l.X, l.Y, l.Z);
        //public static implicit operator Vector4 (Landmark l) => new Vector4(l.X, l.Y, l.Z, l.Confidence);
        public static Landmark operator +(Landmark l) => l;
        public static Landmark operator -(Landmark l) => new Landmark(-l.Position, l.Confidence);
        public static Landmark operator +(Landmark l1, Landmark l2) => new Landmark(l1.Position + l2.Position, Mathf.Min(l1.Confidence, l2.Confidence));
        public static Landmark operator -(Landmark l1, Landmark l2) => l1 + (-l2);
        public static Landmark operator *(Landmark l, float a) => new Landmark(l.Position.x * a, l.Position.y * a, l.Position.z * a, l.Confidence);
    }
}
