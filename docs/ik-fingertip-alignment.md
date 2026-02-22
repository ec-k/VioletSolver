---
created: 2026-02-22T15:34
updated: 2026-02-23T12:00
---
# IK Target Adjustment: Fingertip Alignment Method

This document describes the IK target adjustment method for correctly aligning the index finger during self-touch gestures.

## Background and Challenges

### Problem

Due to body proportion differences between the avatar and the user, self-touch gestures are not reflected correctly. For example, when the user touches their own face, the avatar's hand may not reach the face.

Specifically, the ratio of hand length to arm length, as well as the distance from the neck to the front of the face, tends to be smaller in avatars compared to real humans.

The method described in this document is specifically designed to compensate for differences in hand length.

### Constraints

- FinalIK's VRIK can only set the **wrist position and rotation**. It cannot directly specify the fingertip position.
- Unity Humanoid does not have Tip (fingertip) bones. Distal is the most distal bone available.

### Goal

Align the avatar's index finger Distal to match the index finger DIP position obtained from MediaPipe.Hand.

## Input Data

### Kinect (Pose)

- Provides wrist position and rotation
- Used for full-body pose estimation

### MediaPipe.Hand

- Provides 21 hand landmark points
- index 0: Wrist
- index 7: Index finger DIP (IndexFingerDip)

## Method

### Basic Concept

Since VRIK can only control the wrist, we calculate the wrist position that would place the index finger Distal at the target position (inverse calculation).

Since Kinect's HandTip estimation accuracy is unstable, we use the more reliable results from MediaPipe.Hand.

Since Unity Humanoid does not have Tip bones, we use DIP on the MediaPipe.Hand side instead of Tip, corresponding to the avatar's Distal.

### Implementation Steps

#### Step 1: Coordinate System Unification

Align the MediaPipe.Hand wrist (index 0) to the Kinect wrist position. This unifies the coordinate systems of both sensors.

```
handWristWorld = kinectWristPosition
```

#### Step 2: Calculate Target DIP Position

Add the MediaPipe.Hand wrist-to-DIP offset to the Kinect wrist position to obtain the target DIP position.

```csharp
// MediaPipe.Hand wrist-to-DIP offset
Vector3 userWristToDip = handIndexDip - handWrist;

// Target DIP position (world coordinates)
Vector3 targetIndexDip = poseWristPosition + userWristToDip;
```

#### Step 3: Get Avatar Wrist-to-Distal Vector

Get the vector from the avatar's current wrist to Distal in world coordinates.

```csharp
// Avatar's current wrist-to-Distal vector in world coordinates
Vector3 avatarWristToDistal = distalTransform.position - wristTransform.position;
```

Note: This value is based on the finger state from the previous frame. Since finger movements are relatively slow, this is not a practical issue.

#### Step 4: Calculate IK Wrist Position

Subtract the avatar's wrist-to-Distal vector from the target DIP position to obtain the wrist position.

```csharp
// Wrist position to pass to VRIK
Vector3 ikWristPosition = targetIndexDip - avatarWristToDistal;
```

#### Step 5: Apply to VRIK

Pass the calculated wrist position and the wrist rotation derived from MediaPipe.Hand to VRIK.

```csharp
vrik.solver.leftArm.target.position = ikWristPosition;
vrik.solver.leftArm.target.rotation = wristRotation;
```

## Important Considerations

### One Frame Delay

`avatarWristToDistal` is obtained via `animator.GetBoneTransform`, so it uses the state before the finger rotations calculated in the current frame are applied (one frame behind). Since finger movements are relatively slow, this is not a practical issue.

### Wrist Rotation Consistency

The wrist rotation must also be set correctly, otherwise the index finger will not reach the target position. The wrist rotation needs to be calculated from MediaPipe.Hand and passed to VRIK.

## Formula Summary

```
P_wrist_pose     = Kinect/Pose wrist world position
P_dip_hand       = MediaPipe.Hand DIP position (relative to wrist)
P_wrist_hand     = MediaPipe.Hand wrist position (= origin)

O_user           = P_dip_hand - P_wrist_hand  (user's wrist-to-DIP offset)
P_target_dip     = P_wrist_pose + O_user      (target DIP position)

O_avatar         = Avatar's wrist-to-Distal vector (world coordinates, one frame behind)

P_ik_wrist       = P_target_dip - O_avatar
```

## Related Files

- `Assets/VioletSolver/Runtime/Solvers/FingertipAlignmentSolver.cs`: Fingertip alignment solver
- `Assets/VioletSolver/Runtime/Solvers/HolisticSolver.cs`: Holistic solver
- `Assets/VioletSolver/Runtime/Solvers/HandSolver.cs`: Hand solver
- `Assets/VioletSolver/Runtime/Setup/VRIKSetup.cs`: VRIK initialization

### References
This method is based on the following article:
- [Inverse Kinematics (IK), Applying 2-bone IK to Characters, Replacing Effector Target Position](https://techblog.sega.jp/entry/sega_inverse_kinematics202210)
