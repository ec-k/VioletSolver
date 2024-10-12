using System.Collections.Generic;
using System.Linq;

namespace VioletSolver
{
    public struct HolisticLandmarks
    {
        public PoseLandmarks PoseLandmarks { get; private set; }
        public HandLandmarks LeftHandLandmarks { get; private set; }
        public HandLandmarks RightHandLandmarks { get; private set; }
        public FaceLandmarks FaceLandmarks { get; private set; }

        public HolisticLandmarks(HolisticPose.HolisticLandmarks rowLandmakrs)
        {
            PoseLandmarks = new PoseLandmarks(rowLandmakrs.PoseLandmarks);
            LeftHandLandmarks = new HandLandmarks(rowLandmakrs.LeftHandLandmarks);
            RightHandLandmarks = new HandLandmarks(rowLandmakrs.RightHandLandmarks);
            FaceLandmarks = new FaceLandmarks(rowLandmakrs.FaceLandmarks);
        }

    }

    public class PoseLandmarks : ILandmarks
    {
        List<Landmark> _landmarks;
        public List<Landmark> Landmarks => _landmarks;
        public int Count => Landmarks.Count;
        public PoseLandmarks(int size)
        {
            _landmarks = new Landmark[size].ToList();
        }
        public PoseLandmarks(HolisticPose.LandmarkList rawLandmakrs)
        {
            var count = rawLandmakrs.Landmark.Count;
            _landmarks = new(count);
            for(var i=0;i< count; i++)
            {
                Landmarks[i] = Landmark.SetFromHolisticLandmark(rawLandmakrs.Landmark[i]);
            }
        }
        public void UpdateLandmarks(HolisticPose.LandmarkList rawLandmarks) 
        {
            var count = rawLandmarks.Landmark.Count;
            Utils<Landmark>.ExpandList(count, ref _landmarks);
            for (var i = 0; i < count; i++)
            {
                _landmarks[i] = Landmark.SetFromHolisticLandmark(rawLandmarks.Landmark[i]);
            }
        }
    }
    public struct HandLandmarks : ILandmarks
    {
        public List<Landmark> Landmarks { get; set; }
        public int Count => Landmarks.Count;

        public HandLandmarks(HolisticPose.LandmarkList rowLandmakrs)
        {
            var count = rowLandmakrs.Landmark.Count;
            Landmarks = new(count);
            for (var i = 0; i < count; i++)
            {
                Landmarks[i] = Landmark.SetFromHolisticLandmark(rowLandmakrs.Landmark[i]);
            }
        }
        public void UpdateLandmarks(HolisticPose.LandmarkList landmarks)
        {
            var count = landmarks.Landmark.Count;
            for (var i = 0; i < count; i++)
            {
                Landmarks[i] = Landmark.SetFromHolisticLandmark(landmarks.Landmark[i]);
            }
        }
    }
    public struct FaceLandmarks : ILandmarks
    {
        public List<Landmark> Landmarks { get; set; }
        public int Count => Landmarks.Count;

        // FIXME: functions below are not implement properly because HolisticPose.FaceLandmarks is not defined.
        public FaceLandmarks(HolisticPose.LandmarkList rowLandmakrs)
        {
            //var length = (int)HolisticPose.FasceLandmark.FaceEnd;
            var count = 1; // KILL ERROR NOTIFICATION (implement this lately)
            Landmarks = new(count);
            for (var i = 0; i < count; i++)
            {
                Landmarks[i] = Landmark.SetFromHolisticLandmark(rowLandmakrs.Landmark[i]);
            }
        }
        public void UpdateLandmarks(HolisticPose.LandmarkList landmarks)
        {
            var count = landmarks.Landmark.Count;
            for (var i = 0; i < count; i++)
            {
                Landmarks[i] = Landmark.SetFromHolisticLandmark(landmarks.Landmark[i]);
            }
        }
    }
}
