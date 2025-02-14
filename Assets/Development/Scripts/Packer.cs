using VioletSolver.Landmarks;
using HolisticPose;
using Google.Protobuf.Collections;
namespace VioletSolver.Development
{
    public static class Packer
    {

        public static HolisticLandmarks Convert(IHolisticLandmarks landmarks)
        {
            var holisticResult = new HolisticPose.HolisticLandmarks();
            holisticResult.PoseLandmarks = new HolisticPose.PoseLandmarks();
            holisticResult.LeftHandLandmarks = new HolisticPose.HandLandmarks();
            holisticResult.RightHandLandmarks = new HolisticPose.HandLandmarks();
            holisticResult.FaceResults = new HolisticPose.FaceResults();

            holisticResult.PoseLandmarks.Landmarks.AddRange(Convert(landmarks.Pose));
            holisticResult.LeftHandLandmarks.Landmarks.AddRange(Convert(landmarks.LeftHand));
            holisticResult.RightHandLandmarks.Landmarks.AddRange(Convert(landmarks.RightHand));
            holisticResult.FaceResults.Landmarks.AddRange(Convert(landmarks.Face));

            return holisticResult;
        }

        static RepeatedField<HolisticPose.Landmark> Convert(ILandmarks landmark)
        {
            var lmList = new RepeatedField<HolisticPose.Landmark>();
            for (var i=0;i<landmark.Count;i++)
            {
                lmList.Add(Convert(landmark.Landmarks[i]));
            }
            return lmList;
        }

        static HolisticPose.Landmark Convert (Landmarks.Landmark landmark)
        {
            var lm = new HolisticPose.Landmark();
            lm.X = landmark.X;
            lm.Y = landmark.Y;
            lm.Z = landmark.Z;
            lm.Confidence = landmark.Confidence;
            return lm;
        }
    }
}
