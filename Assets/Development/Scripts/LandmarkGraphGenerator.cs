using poseIndex = HolisticPose.PoseLandmarks.Types.LandmarkIndex;
using handIndex = HolisticPose.HandLandmarks.Types.LandmarkIndex;

namespace VioletSolver.Development
{
    internal static class LandmarkGraphGenerator
    {
        // This connection is based on MediaPipe.Pose
        internal static Graph GeneratePoseGraph()
        {
            Node[] trunk        = { new((int)poseIndex.LeftShoulder),   new((int)poseIndex.RightShoulder),  new((int)poseIndex.RightHip),       new((int)poseIndex.LeftHip) };
            Node[] betweenEars  = { new((int)poseIndex.LeftEar),        new((int)poseIndex.LeftEyeOuter),   new((int)poseIndex.LeftEye),        
                                    new((int)poseIndex.LeftEyeInner),   new((int)poseIndex.Nose),           new((int)poseIndex.RightEyeInner),  new((int)poseIndex.RightEye), 
                                    new((int)poseIndex.RightEyeOuter),  new((int)poseIndex.RightEar) };
            Node[] mouth        = { new((int)poseIndex.MouthLeft),      new((int)poseIndex.MouthRight) };
            Node[] leftHand     = { new((int)poseIndex.LeftIndex),      new((int)poseIndex.LeftPinky),      new((int)poseIndex.LeftWrist),      new((int)poseIndex.LeftThumb) };
            Node[] leftArm      = { new((int)poseIndex.LeftWrist),      new((int)poseIndex.LeftElbow),      new((int)poseIndex.LeftShoulder) };
            Node[] leftLeg      = { new((int)poseIndex.LeftAnkle),      new((int)poseIndex.LeftKnee),       new((int)poseIndex.LeftHip) };
            Node[] leftFoot     = { new((int)poseIndex.LeftFootIndex),  new((int)poseIndex.LeftHeel),       new((int)poseIndex.LeftAnkle) };
            Node[] rightHand    = { new((int)poseIndex.RightIndex),     new((int)poseIndex.RightPinky),     new((int)poseIndex.RightWrist),     new((int)poseIndex.RightThumb) };
            Node[] rightArm     = { new((int)poseIndex.RightWrist),     new((int)poseIndex.RightElbow),     new((int)poseIndex.RightShoulder) };
            Node[] rightLeg     = { new((int)poseIndex.RightAnkle),     new((int)poseIndex.RightKnee),      new((int)poseIndex.RightHip) };
            Node[] rightFoot    = { new((int)poseIndex.RightFootIndex), new((int)poseIndex.RightHeel),      new((int)poseIndex.RightAnkle) };

            return new Graph(trunk, betweenEars, mouth, leftHand, leftArm, leftLeg, leftFoot, rightHand, rightArm, rightFoot, rightLeg);
        }

        // a connection when use Kinect Body Tracking skeleton converted to a MediaPipe.Pose landmark skeleton
        internal static Graph GenerateKinectPoseGraph()
        {
            Node[] trunk        = { new((int)poseIndex.LeftShoulder),   new((int)poseIndex.RightShoulder),  new((int)poseIndex.RightHip), new((int)poseIndex.LeftHip) };
            Node[] leftHand     = { new((int)poseIndex.LeftPinky),      new((int)poseIndex.LeftWrist),      new((int)poseIndex.LeftThumb) };
            Node[] leftArm      = { new((int)poseIndex.LeftWrist),      new((int)poseIndex.LeftElbow),      new((int)poseIndex.LeftShoulder) };
            Node[] leftLeg      = { new((int)poseIndex.LeftAnkle),      new((int)poseIndex.LeftKnee),       new((int)poseIndex.LeftHip) };
            Node[] leftFoot     = { new((int)poseIndex.LeftFootIndex),  new((int)poseIndex.LeftAnkle) };
            Node[] rightHand    = { new((int)poseIndex.RightPinky),     new((int)poseIndex.RightWrist),     new((int)poseIndex.RightThumb) };
            Node[] rightArm     = { new((int)poseIndex.RightWrist),     new((int)poseIndex.RightElbow),     new((int)poseIndex.RightShoulder) };
            Node[] rightLeg     = { new((int)poseIndex.RightAnkle),     new((int)poseIndex.RightKnee),      new((int)poseIndex.RightHip) };
            Node[] rightFoot    = { new((int)poseIndex.RightFootIndex), new((int)poseIndex.RightAnkle) };

            return new Graph(trunk, leftHand, leftArm, leftLeg, leftFoot, rightHand, rightArm, rightFoot, rightLeg);
        }

        // This connection is based on MediaPipe.Hand
        internal static Graph GenerateHandGraph()
        {
            // NOTE:    Since the graph is a tree structure, it might be more flexible and efficient to create a map of the parent nodes and compose links,
            //          but I decided it would be better to enter the data directly for ease of reading.
            Node[] thumb    = { new((int)handIndex.ThumbTip),          new((int)handIndex.ThumbIp),            new((int)handIndex.ThumbMcp), 
                                new((int)handIndex.ThumbCmc),          new((int)handIndex.Wrist) };
            Node[] index    = { new((int)handIndex.IndexFingerTip),    new((int)handIndex.IndexFingerDip),     new((int)handIndex.IndexFingerPip), 
                                new((int)handIndex.IndexFingerMcp),    new((int)handIndex.Wrist) };
            Node[] middle   = { new((int)handIndex.MiddleFingerTip),   new((int)handIndex.MiddleFingerDip),    new((int)handIndex.MiddleFingerPip), 
                                new((int)handIndex.MiddleFingerMcp),   new((int)handIndex.Wrist) };
            Node[] ring     = { new((int)handIndex.RingFingerTip),     new((int)handIndex.RingFingerDip),      new((int)handIndex.RingFingerPip), 
                                new((int)handIndex.RingFingerMcp),     new((int)handIndex.Wrist) };
            Node[] pinky    = { new((int)handIndex.PinkyTip),          new((int)handIndex.PinkyDip),           new((int)handIndex.PinkyPip), 
                                new((int)handIndex.PinkyMcp),          new((int)handIndex.Wrist) };

            return new Graph(thumb, index, middle, ring, pinky);
        }
    }
}
