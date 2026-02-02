namespace VioletSolver.Landmarks
{
    /// <summary>
    /// Holds pose, hand, and face landmarks together.
    /// </summary>
    public class HolisticLandmarks: IHolisticLandmarks
    {
        PoseLandmarks _pose;
        LandmarkList _leftHand;
        LandmarkList _rightHand;
        LandmarkList _face;

        public ILandmarkList Pose      => _pose.KinectResult;   // HACK: this code is just for implementig interface
        public ILandmarkList KinectPose => _pose.KinectResult;
        public ILandmarkList MediaPipePose => _pose.MediaPipeResult;
        public ILandmarkList LeftHand  => _leftHand;
        public ILandmarkList RightHand => _rightHand;
        public ILandmarkList Face      => _face;

        /// <summary>
        /// Initializes a new HolisticLandmarks instance.
        /// </summary>
        /// <param name="faceLandmarkLength">The number of face landmarks.</param>
        public HolisticLandmarks(int faceLandmarkLength)
        {
            var kinectPoseLength = (int)HumanLandmarks.KinectPoseLandmarks.Types.LandmarkIndex.Length;
            var mediaPipePoseLength = (int)HumanLandmarks.MediaPipePoseLandmarks.Types.LandmarkIndex.Length;
            var handLength = (int)HumanLandmarks.HandLandmarks.Types.LandmarkIndex.Length;

            _pose = new()
            {
                KinectResult = new(kinectPoseLength),
                MediaPipeResult = new(mediaPipePoseLength),
            };
            _leftHand  = new LandmarkList(handLength);
            _rightHand = new LandmarkList(handLength);
            _face      = new LandmarkList(faceLandmarkLength);
        }

        /// <summary>
        /// Updates each landmark list with the received landmark data.
        /// </summary>
        /// <param name="landmarks">The received HolisticLandmarks data.</param>
        /// <param name="time">The time the data was received.</param>
        public void UpdateLandmarks(HumanLandmarks.HolisticLandmarks landmarks, float time)
        {
            var existKinectPose    = landmarks.KinectPoseLandmarks    != null;
            var existMediaPipePose = landmarks.MediaPipePoseLandmarks != null;
            var existLefthand      = landmarks.LeftHandLandmarks      != null;
            var existRightHand     = landmarks.RightHandLandmarks     != null;
            var existFace          = landmarks.FaceResults            != null;

            if (existKinectPose)    _pose.KinectResult.Set(landmarks.KinectPoseLandmarks.Landmarks, time);
            if (existMediaPipePose) _pose.MediaPipeResult.Set(landmarks.MediaPipePoseLandmarks.Landmarks, time);
            if (existLefthand)      _leftHand.Set(landmarks.LeftHandLandmarks.Landmarks, time);
            if (existRightHand)     _rightHand.Set(landmarks.RightHandLandmarks.Landmarks, time);
            if (existFace)          _face.Set(landmarks.FaceResults.Landmarks, time);
        }
    }

    internal class PoseLandmarks
    {
        internal LandmarkList KinectResult { get; set; }
        internal LandmarkList MediaPipeResult { get; set; }
    }
}
