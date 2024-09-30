using UnityEngine;

namespace VioletSolver
{
    public interface IHolisticLandmarks
    {
        public ILandmarks Pose { get; set; }
        public ILandmarks LeftHand { get; set; }
        public ILandmarks RightHand { get; set; }
        public ILandmarks Face { get; set; }

        public void UpdateLandmarks(HolisticPose.HolisticLandmarks landmarks);
    }

    public interface ILandmarks
    {
        public Landmark[] Landmarks { get; set; }
        public int Count => Landmarks.Length;
        public void UpdateLandmark(HolisticPose.LandmarkList landmarks);
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

        public void SetFromHolisticLandmark(HolisticPose.Landmark landmark)
        {
            _position = new Vector3(
                landmark.X,
                landmark.Y,
                landmark.Z);
            Confidence = landmark.Confidence;
        }

    }
}
