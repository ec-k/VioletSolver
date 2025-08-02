using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VioletSolver.Landmarks
{
    internal struct HolisticLandmarks: IHolisticLandmarks
    {
        public ILandmarks Pose { get; set; }
        public ILandmarks LeftHand { get; set; }
        public ILandmarks RightHand { get; set; }
        public ILandmarks Face { get; set; }

        public HolisticLandmarks(int faceLandmarkLength)
        {
            var poseLength = (int)HumanLandmarks.KinectPoseLandmarks.Types.LandmarkIndex.Length;
            var handLength = (int)HumanLandmarks.HandLandmarks.Types.LandmarkIndex.Length;

            Pose = new PoseLandmarks(poseLength);
            LeftHand = new HandLandmarks(handLength);
            RightHand = new HandLandmarks(handLength);
            Face = new FaceLandmarks(faceLandmarkLength);
        }
        public HolisticLandmarks(HumanLandmarks.HolisticLandmarks rowLandmakrs)
        {
            Pose = new PoseLandmarks(rowLandmakrs.PoseLandmarks);
            LeftHand = new HandLandmarks(rowLandmakrs.LeftHandLandmarks);
            RightHand = new HandLandmarks(rowLandmakrs.RightHandLandmarks);
            Face = new FaceLandmarks(rowLandmakrs.FaceResults);
        }

        public void UpdateLandmarks(HumanLandmarks.HolisticLandmarks landmarks, float time)
        {
            var existPose       = landmarks.PoseLandmarks       != null;
            var existLefthand   = landmarks.LeftHandLandmarks   != null;
            var existRightHand  = landmarks.RightHandLandmarks  != null;
            var existFace       = landmarks.FaceResults         != null;

            if (existPose)      Pose.UpdateLandmarks(landmarks.PoseLandmarks.Landmarks, time);
            if (existLefthand)  LeftHand.UpdateLandmarks(landmarks.LeftHandLandmarks.Landmarks, time);
            if (existRightHand) RightHand.UpdateLandmarks(landmarks.RightHandLandmarks.Landmarks, time);
            if (existFace)      Face.UpdateLandmarks(landmarks.FaceResults.Landmarks, time);
        }


    }

    internal struct PoseLandmarks : ILandmarks
    {
        List<Landmark> _landmarks;
        public List<Landmark> Landmarks => _landmarks;
        public int Count => Landmarks.Count;
        float _time;
        public float Time => _time;
        internal PoseLandmarks(int size)
        {
            _landmarks = new Landmark[size].ToList();
            _time = 0f;
        }
        internal PoseLandmarks(HumanLandmarks.PoseLandmarks rawLandmakrs)
        {
            var count = rawLandmakrs.Landmarks.Count;
            _landmarks = new(count);
            for(var i=0;i < count; i++)
            {
                _landmarks[i] = Landmark.SetFromHolisticLandmark(rawLandmakrs.Landmarks[i]);
            }
            _time = 0f;
        }
        internal PoseLandmarks(HumanLandmarks.KinectPoseLandmarks rawLandmakrs)
        {
            var count = rawLandmakrs.Landmarks.Count;
            _landmarks = new(count);
            for (var i = 0; i < count; i++)
            {
                _landmarks[i] = Landmark.SetFromHolisticLandmark(rawLandmakrs.Landmarks[i]);
            }
            _time = 0f;
        }
        public void UpdateLandmarks(RepeatedField<HumanLandmarks.Landmark> rawLandmarks, float time) 
        {
            for (var i = 0; i < Count; i++)
            {
                _landmarks[i] = Landmark.SetFromHolisticLandmark(rawLandmarks[i]);
            }
            _time = time;
        }
    }
    internal struct HandLandmarks : ILandmarks
    {
        List<Landmark> _landmarks;
        public List<Landmark> Landmarks => _landmarks;
        public int Count => Landmarks.Count;
        float _time;
        public float Time => _time;
        internal HandLandmarks(int size)
        {
            _landmarks = new Landmark[size].ToList();
            _time = 0f;
        }

        internal HandLandmarks(HumanLandmarks.HandLandmarks rowLandmakrs)
        {
            var count = rowLandmakrs.Landmarks.Count;
            _landmarks = new(count);
            for (var i = 0; i < count; i++)
            {
                _landmarks[i] = Landmark.SetFromHolisticLandmark(rowLandmakrs.Landmarks[i]);
            }
            _time = 0f;
        }
        public void UpdateLandmarks(RepeatedField<HumanLandmarks.Landmark> rawLandmarks, float time)
        {
            for (var i = 0; i < Count; i++)
            {
                _landmarks[i] = Landmark.SetFromHolisticLandmark(rawLandmarks[i]);
            }
            _time = time;
        }
    }
    internal struct FaceLandmarks : ILandmarks
    {
        List<Landmark> _landmarks;
        public List<Landmark> Landmarks => _landmarks;
        public int Count => Landmarks.Count;
        float _time;
        public float Time => _time;
        internal FaceLandmarks(int size)
        {
            _landmarks = new Landmark[size].ToList();
            _time = 0f;
        }
        internal FaceLandmarks(HumanLandmarks.FaceResults faceResults)
        {
            var count = faceResults.Landmarks.Count;
            _landmarks = new(count);
            for (var i = 0; i < count; i++)
            {
                _landmarks[i] = Landmark.SetFromHolisticLandmark(faceResults.Landmarks[i]);
            }
            _time = 0f;
        }
        public void UpdateLandmarks(RepeatedField<HumanLandmarks.Landmark> rawLandmarks, float time)
        {
            for (var i = 0; i < Count; i++)
            {
                _landmarks[i] = Landmark.SetFromHolisticLandmark(rawLandmarks[i]);
            }
            _time = time;
        }
    }
}
