using Google.Protobuf.Collections;
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
            PoseLandmarks = new PoseLandmarks(rowLandmakrs.poseLandmarks.Landmarks);
            LeftHandLandmarks = new HandLandmarks(rowLandmakrs.LeftHandLandmarks.Landmarks);
            RightHandLandmarks = new HandLandmarks(rowLandmakrs.RightHandLandmarks.Landmarks);
            FaceLandmarks = new FaceLandmarks(rowLandmakrs.FaceResults.Landmarks);
        }

    }

    public struct PoseLandmarks : ILandmarks
    {
        List<Landmark> _landmarks;
        public List<Landmark> Landmarks => _landmarks;
        public int Count => Landmarks.Count;
        public PoseLandmarks(int size)
        {
            _landmarks = new Landmark[size].ToList();
        }
        public PoseLandmarks(RepeatedField<HolisticPose.Landmark> rawLandmakrs)
        {
            var count = rawLandmakrs.Count;
            _landmarks = new(count);
            for(var i=0;i< count; i++)
            {
                Landmarks[i] = Landmark.SetFromHolisticLandmark(rawLandmakrs[i]);
            }
        }
        public void UpdateLandmarks(RepeatedField<HolisticPose.Landmark> rawLandmarks) 
        {
            var count = rawLandmarks.Count;
            Utils<Landmark>.ExpandList(count, ref _landmarks);
            for (var i = 0; i < count; i++)
            {
                _landmarks[i] = Landmark.SetFromHolisticLandmark(rawLandmarks[i]);
            }
        }
    }
    public struct HandLandmarks : ILandmarks
    {
        public List<Landmark> Landmarks { get; set; }
        public int Count => Landmarks.Count;

        public HandLandmarks(RepeatedField<HolisticPose.Landmark> rowLandmakrs)
        {
            var count = rowLandmakrs.Count;
            Landmarks = new(count);
            for (var i = 0; i < count; i++)
            {
                Landmarks[i] = Landmark.SetFromHolisticLandmark(rowLandmakrs[i]);
            }
        }
        public void UpdateLandmarks(RepeatedField<HolisticPose.Landmark> landmarks)
        {
            var count = landmarks.Count;
            for (var i = 0; i < count; i++)
            {
                Landmarks[i] = Landmark.SetFromHolisticLandmark(landmarks[i]);
            }
        }
    }
    public struct FaceLandmarks : ILandmarks
    {
        public List<Landmark> Landmarks { get; set; }
        public int Count => Landmarks.Count;

        // FIXME: functions below are not implement properly because HolisticPose.FaceLandmarks is not defined.
        public FaceLandmarks(RepeatedField<HolisticPose.Landmark> rowLandmakrs)
        {
            //var length = (int)HolisticPose.FasceLandmark.FaceEnd;
            var count = 1; // KILL ERROR NOTIFICATION (implement this lately)
            Landmarks = new(count);
            for (var i = 0; i < count; i++)
            {
                Landmarks[i] = Landmark.SetFromHolisticLandmark(rowLandmakrs[i]);
            }
        }
        public void UpdateLandmarks(RepeatedField<HolisticPose.Landmark> landmarks)
        {
            var count = landmarks.Count;
            for (var i = 0; i < count; i++)
            {
                Landmarks[i] = Landmark.SetFromHolisticLandmark(landmarks[i]);
            }
        }
    }
}
