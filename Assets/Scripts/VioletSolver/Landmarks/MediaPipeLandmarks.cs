using Google.Protobuf.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VioletSolver
{
    public struct HolisticLandmarks: IHolisticLandmarks
    {
        public ILandmarks Pose { get; set; }
        public ILandmarks LeftHand { get; set; }
        public ILandmarks RightHand { get; set; }
        public ILandmarks Face { get; set; }

        public HolisticLandmarks(int size)
        {
            Pose = new PoseLandmarks(size);
            LeftHand = new HandLandmarks(size);
            RightHand = new HandLandmarks(size);
            Face = new FaceLandmarks(500);
        }
        public HolisticLandmarks(HolisticPose.HolisticLandmarks rowLandmakrs)
        {
            Pose = new PoseLandmarks(rowLandmakrs.poseLandmarks.Landmarks);
            LeftHand = new HandLandmarks(rowLandmakrs.LeftHandLandmarks.Landmarks);
            RightHand = new HandLandmarks(rowLandmakrs.RightHandLandmarks.Landmarks);
            Face = new FaceLandmarks(rowLandmakrs.FaceResults.Landmarks);
        }

        public void UpdateLandmarks(HolisticPose.HolisticLandmarks landmarks, float time)
        {
            var existPose       = landmarks.poseLandmarks       != null;
            var existLefthand   = landmarks.LeftHandLandmarks   != null;
            var existRightHand  = landmarks.RightHandLandmarks  != null;
            var existFace       = landmarks.FaceResults         != null;

            if (existPose)      Pose.UpdateLandmarks(landmarks.poseLandmarks.Landmarks, time);
            if (existLefthand)  LeftHand.UpdateLandmarks(landmarks.LeftHandLandmarks.Landmarks, time);
            if (existRightHand) RightHand.UpdateLandmarks(landmarks.RightHandLandmarks.Landmarks, time);
            if (existFace)      Face.UpdateLandmarks(landmarks.FaceResults.Landmarks, time);
        }


    }

    public struct PoseLandmarks : ILandmarks
    {
        List<Landmark> _landmarks;
        public List<Landmark> Landmarks => _landmarks;
        public int Count => Landmarks.Count;
        float _time;
        public float Time => _time;
        public PoseLandmarks(int size)
        {
            _landmarks = new Landmark[size].ToList();
            _time = 0f;
        }
        public PoseLandmarks(RepeatedField<HolisticPose.Landmark> rawLandmakrs)
        {
            var count = rawLandmakrs.Count;
            _landmarks = new(count);
            for(var i=0;i< count; i++)
            {
                _landmarks[i] = Landmark.SetFromHolisticLandmark(rawLandmakrs[i]);
            }
            _time = 0f;
        }
        public void UpdateLandmarks(RepeatedField<HolisticPose.Landmark> rawLandmarks, float time) 
        {
            var count = rawLandmarks.Count;
            Utils<Landmark>.ExpandList(count, ref _landmarks);
            for (var i = 0; i < count; i++)
            {
                _landmarks[i] = Landmark.SetFromHolisticLandmark(rawLandmarks[i]);
            }
            _time = time;
        }
    }
    public struct HandLandmarks : ILandmarks
    {
        List<Landmark> _landmarks;
        public List<Landmark> Landmarks => _landmarks;
        public int Count => Landmarks.Count;
        float _time;
        public float Time => _time;
        public HandLandmarks(int size)
        {
            _landmarks = new Landmark[size].ToList();
            _time = 0f;
        }

        public HandLandmarks(RepeatedField<HolisticPose.Landmark> rowLandmakrs)
        {
            var count = rowLandmakrs.Count;
            _landmarks = new(count);
            for (var i = 0; i < count; i++)
            {
                _landmarks[i] = Landmark.SetFromHolisticLandmark(rowLandmakrs[i]);
            }
            _time = 0f;
        }
        public void UpdateLandmarks(RepeatedField<HolisticPose.Landmark> rawLandmarks, float time)
        {
            var count = rawLandmarks.Count;
            Utils<Landmark>.ExpandList(count, ref _landmarks);
            for (var i = 0; i < count; i++)
            {
                _landmarks[i] = Landmark.SetFromHolisticLandmark(rawLandmarks[i]);
            }
            _time = time;
        }
    }
    public struct FaceLandmarks : ILandmarks
    {
        List<Landmark> _landmarks;
        public List<Landmark> Landmarks => _landmarks;
        public int Count => Landmarks.Count;
        float _time;
        public float Time => _time;
        public FaceLandmarks(int size)
        {
            _landmarks = new Landmark[size].ToList();
            _time = 0f;
        }
        public FaceLandmarks(RepeatedField<HolisticPose.Landmark> rowLandmakrs)
        {
            var count = rowLandmakrs.Count;
            _landmarks = new(count);
            for (var i = 0; i < count; i++)
            {
                _landmarks[i] = Landmark.SetFromHolisticLandmark(rowLandmakrs[i]);
            }
            _time = 0f;
        }
        public void UpdateLandmarks(RepeatedField<HolisticPose.Landmark> landmarks, float time)
        {
            var count = landmarks.Count;
            Utils<Landmark>.ExpandList(count, ref _landmarks);
            for (var i = 0; i < count; i++)
            {
                _landmarks[i] = Landmark.SetFromHolisticLandmark(landmarks[i]);
            }
            _time = time;
        }
    }
}
