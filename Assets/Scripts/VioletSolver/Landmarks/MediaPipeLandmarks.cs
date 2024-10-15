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
            Face = new FaceLandmarks(size);
        }
        public HolisticLandmarks(HolisticPose.HolisticLandmarks rowLandmakrs)
        {
            Pose = new PoseLandmarks(rowLandmakrs.poseLandmarks.Landmarks);
            LeftHand = new HandLandmarks(rowLandmakrs.LeftHandLandmarks.Landmarks);
            RightHand = new HandLandmarks(rowLandmakrs.RightHandLandmarks.Landmarks);
            Face = new FaceLandmarks(rowLandmakrs.FaceResults.Landmarks);
        }

        public void UpdateLandmarks(HolisticPose.HolisticLandmarks landmarks)
        {
            Pose.UpdateLandmarks(landmarks.poseLandmarks.Landmarks);
            //LeftHand.UpdateLandmarks(landmarks.LeftHandLandmarks.Landmarks);
            //RightHand.UpdateLandmarks(landmarks.RightHandLandmarks.Landmarks);
            //Face.UpdateLandmarks(landmarks.FaceResults.Landmarks);
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
        List<Landmark> _landmarks;
        public List<Landmark> Landmarks => _landmarks;
        public int Count => Landmarks.Count;
        public HandLandmarks(int size)
        {
            _landmarks = new Landmark[size].ToList();
        }

        public HandLandmarks(RepeatedField<HolisticPose.Landmark> rowLandmakrs)
        {
            var count = rowLandmakrs.Count;
            _landmarks = new(count);
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
                _landmarks[i] = Landmark.SetFromHolisticLandmark(landmarks[i]);
            }
        }
    }
    public struct FaceLandmarks : ILandmarks
    {
        List<Landmark> _landmarks;
        public List<Landmark> Landmarks => _landmarks;
        public int Count => Landmarks.Count;
        public FaceLandmarks(int size)
        {
            _landmarks = new Landmark[size].ToList();
        }

        // FIXME: functions below are not implement properly because HolisticPose.FaceLandmarks is not defined.
        public FaceLandmarks(RepeatedField<HolisticPose.Landmark> rowLandmakrs)
        {
            //var length = (int)HolisticPose.FasceLandmark.FaceEnd;
            var count = 1; // KILL ERROR NOTIFICATION (implement this lately)
            _landmarks = new(count);
            for (var i = 0; i < count; i++)
            {
                _landmarks[i] = Landmark.SetFromHolisticLandmark(rowLandmakrs[i]);
            }
        }
        public void UpdateLandmarks(RepeatedField<HolisticPose.Landmark> landmarks)
        {
            var count = landmarks.Count;
            for (var i = 0; i < count; i++)
            {
                _landmarks[i] = Landmark.SetFromHolisticLandmark(landmarks[i]);
            }
        }
    }
}
