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

    public struct PoseLandmarks : ILandmarks
    {
        public Landmark[] Landmarks { get; set; }
        public int Count => Landmarks.Length;
        public PoseLandmarks(HolisticPose.LandmarkList rowLandmakrs)
        {
            var length = (int)HolisticPose.PoseLandmark.PoseEnd;
            Landmarks = new Landmark[length];
            for(var i=0;i< length; i++)
            {
                Landmarks[i].SetFromHolisticLandmark(rowLandmakrs.Landmark[i]);
            }
        }
        public void UpdateLandmark(HolisticPose.LandmarkList landmarks) 
        {
            var length = (int)HolisticPose.PoseLandmark.PoseEnd;
            for (var i = 0; i < length; i++)
            {
                Landmarks[i].SetFromHolisticLandmark(landmarks.Landmark[i]);
            }
        }
    }
    public struct HandLandmarks : ILandmarks
    {
        public Landmark[] Landmarks { get; set; }
        public int Count => Landmarks.Length;

        public HandLandmarks(HolisticPose.LandmarkList rowLandmakrs)
        {
            var length = (int)HolisticPose.HandLandmark.HandEnd;
            Landmarks = new Landmark[length];
            for (var i = 0; i < length; i++)
            {
                Landmarks[i].SetFromHolisticLandmark(rowLandmakrs.Landmark[i]);
            }
        }
        public void UpdateLandmark(HolisticPose.LandmarkList landmarks)
        {
            var length = (int)HolisticPose.HandLandmark.HandEnd;
            for (var i = 0; i < length; i++)
            {
                Landmarks[i].SetFromHolisticLandmark(landmarks.Landmark[i]);
            }
        }
    }
    public struct FaceLandmarks : ILandmarks
    {
        public Landmark[] Landmarks { get; set; }
        public int Count => Landmarks.Length;

        // FIXME: functions below are not implement properly because HolisticPose.FaceLandmarks is not defined.
        public FaceLandmarks(HolisticPose.LandmarkList rowLandmakrs)
        {
            //var length = (int)HolisticPose.FasceLandmark.FaceEnd;
            var length = 1; // KILL ERROR NOTIFICATION (implement this lately)
            Landmarks = new Landmark[length];
            for (var i = 0; i < length; i++)
            {
                Landmarks[i].SetFromHolisticLandmark(rowLandmakrs.Landmark[i]);
            }
        }
        public void UpdateLandmark(HolisticPose.LandmarkList landmarks)
        {
            var length = (int)HolisticPose.PoseLandmark.PoseEnd;
            for (var i = 0; i < length; i++)
            {
                Landmarks[i].SetFromHolisticLandmark(landmarks.Landmark[i]);
            }
        }
    }
}
