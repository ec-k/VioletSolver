using Google.Protobuf.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VioletSolver
{
    public interface IHolisticLandmarks
    {
        public ILandmarks Pose { get; set; }
        public ILandmarks LeftHand { get; set; }
        public ILandmarks RightHand { get; set; }
        public ILandmarks Face { get; set; }

        public void UpdateLandmarks(HolisticPose.HolisticLandmarks landmarks, float time);
    }

    public interface ILandmarks
    {
        public List<Landmark> Landmarks { get; }
        public int Count => Landmarks.Count;
        public float Time { get; }
        public void UpdateLandmarks(RepeatedField<HolisticPose.Landmark> landmarks, float time);
    }

    public struct Landmark
    {
        Vector3 _position;
        public Vector3 Position => _position;
        public float X => _position.x;
        public float Y => _position.y;
        public float Z => _position.z;
        public float Confidence { get; set; }

        public Landmark(HolisticPose.Landmark landmark)
        {
            _position = new Vector3(
                landmark.X,
                landmark.Y,
                landmark.Z);
            Confidence = landmark.Confidence;
        }
        public Landmark(Vector3 position, float confidence)
        {
            _position = position;
            Confidence = confidence;
        }
        public Landmark(float x, float y, float z)
        {
            _position = new Vector3(x, y, z);
            Confidence = 0.0f;
        }
        public Landmark(float x, float y, float z, float confidence)
        {
            _position = new Vector3(x, y, z);
            Confidence = confidence;
        }

        public static Landmark SetFromHolisticLandmark(HolisticPose.Landmark landmark)
        {
            return new Landmark(
                landmark.X,
                landmark.Y,
                landmark.Z,
                landmark.Confidence);
        }

        public static Landmark Lerp(Landmark l1,  Landmark l2, float amount)
        =>  new Landmark(
                Mathf.Lerp(l1.X, l2.X, amount),
                Mathf.Lerp(l1.Y, l2.Y, amount),
                Mathf.Lerp(l1.Z, l2.Z, amount),
                Mathf.Lerp(l1.Confidence, l2.Confidence, amount));

        public static implicit operator Vector3 (Landmark l) => new Vector3(l.X, l.Y, l.Z);
        public static implicit operator Vector4 (Landmark l) => new Vector4(l.X, l.Y, l.Z, l.Confidence);
        public static Landmark operator +(Landmark l) => l;
        public static Landmark operator -(Landmark l) => new Landmark(-l.Position, l.Confidence);
        public static Landmark operator +(Landmark l1, Landmark l2) => new Landmark(l1.Position + l2.Position, Mathf.Min(l1.Confidence, l2.Confidence));
        public static Landmark operator -(Landmark l1, Landmark l2) => l1 + (-l2);
    }
}
