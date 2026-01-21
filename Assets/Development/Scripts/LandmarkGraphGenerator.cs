using poseIndex = HumanLandmarks.MediaPipePoseLandmarks.Types.LandmarkIndex;
using kinectPoseIndex = HumanLandmarks.KinectPoseLandmarks.Types.LandmarkIndex;
using handIndex = HumanLandmarks.HandLandmarks.Types.LandmarkIndex;

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

        // a connection when use Kinect Body Tracking skeleton
        internal static Graph GenerateKinectPoseGraph()
        {
            Node[] face         = { new((int)kinectPoseIndex.EarLeft),        new((int)kinectPoseIndex.EyeLeft),        new((int)kinectPoseIndex.Nose),          new((int)kinectPoseIndex.EyeRight),      new((int)kinectPoseIndex.EarRight) };
            Node[] trunk        = { new((int)kinectPoseIndex.Pelvis),         new((int)kinectPoseIndex.SpineNaval),     new((int)kinectPoseIndex.SpineChest),    new((int)kinectPoseIndex.Neck),          new((int)kinectPoseIndex.Head) };
            Node[] leftHand     = { new((int)kinectPoseIndex.HandtipLeft),    new((int)kinectPoseIndex.WristLeft),      new((int)kinectPoseIndex.ThumbLeft),     new((int)kinectPoseIndex.ClavicleLeft),  new((int)kinectPoseIndex.SpineChest) };
            Node[] leftArm      = { new((int)kinectPoseIndex.WristLeft),      new((int)kinectPoseIndex.ElbowLeft),      new((int)kinectPoseIndex.ShoulderLeft) };
            Node[] leftLeg      = { new((int)kinectPoseIndex.AnkleLeft),      new((int)kinectPoseIndex.KneeLeft),       new((int)kinectPoseIndex.HipLeft),       new((int)kinectPoseIndex.Pelvis) };
            Node[] leftFoot     = { new((int)kinectPoseIndex.FootLeft),       new((int)kinectPoseIndex.AnkleLeft) };
            Node[] rightHand    = { new((int)kinectPoseIndex.HandtipRight),   new((int)kinectPoseIndex.WristRight),     new((int)kinectPoseIndex.ThumbRight) };
            Node[] rightArm     = { new((int)kinectPoseIndex.WristRight),     new((int)kinectPoseIndex.ElbowRight),     new((int)kinectPoseIndex.ShoulderRight), new((int)kinectPoseIndex.ClavicleRight), new((int)kinectPoseIndex.SpineChest) };
            Node[] rightLeg     = { new((int)kinectPoseIndex.AnkleRight),     new((int)kinectPoseIndex.KneeRight),      new((int)kinectPoseIndex.HipRight),      new((int)kinectPoseIndex.Pelvis) };
            Node[] rightFoot    = { new((int)kinectPoseIndex.FootRight),      new((int)kinectPoseIndex.AnkleRight) };

            return new Graph(face, trunk, leftHand, leftArm, leftLeg, leftFoot, rightHand, rightArm, rightFoot, rightLeg);
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
