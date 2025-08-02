using VioletSolver.Landmarks;
using HumanLandmarks;
using Google.Protobuf.Collections;
namespace VioletSolver.Development
{
    public static class Packer
    {

        public static HolisticLandmarks Convert(IHolisticLandmarks landmarks)
        {
            var holisticResult = new HolisticLandmarks();
            holisticResult.PoseLandmarks = new KinectPoseLandmarks();
            holisticResult.LeftHandLandmarks = new HandLandmarks();
            holisticResult.RightHandLandmarks = new HandLandmarks();
            holisticResult.FaceResults = new FaceResults();

            holisticResult.PoseLandmarks.Landmarks.AddRange(Convert(landmarks.Pose));
            holisticResult.LeftHandLandmarks.Landmarks.AddRange(Convert(landmarks.LeftHand));
            holisticResult.RightHandLandmarks.Landmarks.AddRange(Convert(landmarks.RightHand));
            holisticResult.FaceResults.Landmarks.AddRange(Convert(landmarks.Face));

            return holisticResult;
        }

        static RepeatedField<HumanLandmarks.Landmark> Convert(ILandmarks landmark)
        {
            var lmList = new RepeatedField<HumanLandmarks.Landmark>();
            for (var i=0;i<landmark.Count;i++)
            {
                lmList.Add(Convert(landmark.Landmarks[i]));
            }
            return lmList;
        }

        static HumanLandmarks.Landmark Convert (Landmarks.Landmark landmark)
        {
            var lm = new HumanLandmarks.Landmark();
            lm.Position.X = landmark.Position.x;
            lm.Position.Y = landmark.Position.y;
            lm.Position.Z = landmark.Position.z;
            lm.Confidence = landmark.Confidence;
            return lm;
        }
    }
}
