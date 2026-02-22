---
created: 2026-02-22T15:34
updated: 2026-02-22T15:43
---
# IK Target Adjustment: Fingertip Alignment Method

This document describes the IK target adjustment method for correctly aligning the index fingertip during self-touch gestures.

## Background and Challenges

### Problem

Due to body proportion differences between the avatar and the user, self-touch gestures are not reflected correctly. For example, when the user touches their own face, the avatar's hand may not reach the face.

Specifically, the ratio of hand length to arm length, as well as the distance from the neck to the front of the face, tends to be smaller in avatars compared to real humans.

The method described in this document is specifically designed to compensate for differences in hand length.

### Constraints

FinalIK's VRIK can only set the **wrist position and rotation**. It cannot directly specify the fingertip position.

### Goal

Align the avatar's index fingertip to match the index fingertip position obtained from MediaPipe.Hand.

## Input Data

### Kinect (Pose)

- Provides wrist position and rotation
- Used for full-body pose estimation

### MediaPipe.Hand

- Provides 21 hand landmark points
- index 0: Wrist
- index 8: Index fingertip (IndexFingerTip)

## Method

### Basic Concept

Since VRIK can only control the wrist, we calculate the wrist position that would place the index fingertip at the target position (inverse calculation).

However, since Kinect's HandTip estimation accuracy is unstable, we use the more reliable results from MediaPipe.Hand.

### Implementation Steps

#### Step 1: Coordinate System Unification

Align the MediaPipe.Hand wrist (index 0) to the Kinect wrist position. This unifies the coordinate systems of both sensors.

```
handWristWorld = kinectWristPosition
```

#### Step 2: Calculate Offset Considering Wrist Rotation

Calculate the avatar's "wrist to index fingertip offset" considering the current wrist rotation.

```csharp
// Offset in avatar's local space (bind pose or current pose)
Vector3 localOffset = avatarIndexTip - avatarWrist;  // Local space

// Transform to world space by applying current wrist rotation
Vector3 rotatedOffset = wristRotation * localOffset;
```

#### Step 3: Calculate IK Wrist Position

Subtract the rotated offset from the MediaPipe.Hand index fingertip position to obtain the wrist position.

```csharp
// MediaPipe.Hand index fingertip (converted to world coordinates)
Vector3 targetIndexTip = TransformToWorld(mediaPipeHand.Landmarks[8]);

// Wrist position to pass to VRIK
Vector3 ikWristPosition = targetIndexTip - rotatedOffset;
```

#### Step 4: Apply to VRIK

Pass the calculated wrist position and the wrist rotation derived from MediaPipe.Hand to VRIK.

```csharp
vrik.solver.leftArm.target.position = ikWristPosition;
vrik.solver.leftArm.target.rotation = wristRotation;
```

## Important Considerations

### Use Pre-IK Values

The calculation of `avatarIndexTip - avatarWrist` must use bone positions before IK is applied. Using post-IK values will cause a feedback loop.

Countermeasures:
- Calculate in VRIK's `OnPreUpdate`
- Or cache the bone positions before IK application

### Wrist Rotation Consistency

The wrist rotation must also be set correctly, otherwise the index fingertip will not reach the target position. The wrist rotation needs to be calculated from MediaPipe.Hand and passed to VRIK.

### Scale Correction

When there are body proportion differences between the avatar and user, scale correction may be needed for the local offset.

```csharp
Vector3 scaledLocalOffset = localOffset * avatarScale;
```

## Formula Summary

```
P_target    = World position of MediaPipe.Hand index fingertip
R_wrist     = Wrist rotation (from MediaPipe.Hand or Kinect)
O_local     = Local offset from avatar's wrist to index fingertip
O_rotated   = R_wrist * O_local

P_ikWrist   = P_target - O_rotated
```

## Related Files

- `Assets/VioletSolver/Runtime/Solvers/KinectPoseSolver.cs`: Kinect pose solver
- `Assets/VioletSolver/Runtime/Solvers/HandSolver.cs`: Hand solver
- `Assets/VioletSolver/Runtime/Setup/VRIKSetup.cs`: VRIK initialization

### References
This method is based on the following article:
- [Inverse Kinematics (IK), Applying 2-bone IK to Characters, Replacing Effector Target Position](https://techblog.sega.jp/entry/sega_inverse_kinematics202210)
