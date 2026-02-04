# VioletSolver Runtime Architecture

This document describes the architecture and data flow of the VioletSolver runtime.

## Pipeline Overview

VioletSolver processes landmark data from motion capture sources and applies it to humanoid avatars. The data flows through the following pipeline:

```
LandmarkProvider -> Landmark -> Solver -> Pose -> AvatarAnimator
                                                        |
                                                  Interpolation
                                                  (frame rate adaptation)
```

## Directory Structure

```
Runtime/
├── LandmarkProviders/   # Input sources
├── Landmark/            # Landmark data management
├── Solvers/             # Landmark to pose conversion
├── Pose/                # Pose data structures
├── FaceApplier/         # Facial expression application
├── Interpolation/       # Frame rate adaptation
├── Setup/               # Initialization and calibration
├── Utils/               # General utilities
├── AvatarAnimator.cs    # Main component (facade)
└── AnimationResultData.cs
```

## Directory Roles

### LandmarkProviders/

Provides landmark data from various input sources (e.g., MediaPipe, Kinect). Implements `ILandmarkProvider` interface to abstract different motion capture systems.

### Landmark/

Manages and processes raw landmark data. Contains:
- `LandmarkHandler`: Filters and manages landmark data
- `HolisticLandmarks`: Combined landmark data structure
- `LandmarksData`: Base landmark data types

### Solvers/

Converts landmark positions to avatar pose rotations. Contains:
- `HolisticSolver`: Main solver that orchestrates pose calculation
- `MediaPipePoseSolver`: Solver for MediaPipe landmarks
- `KinectPoseSolver`: Solver for Kinect landmarks
- `HandSolver`: Finger pose calculation
- `Face/`: Blendshape solvers for facial expressions
- `RestPose/`: Avatar rest pose data management

### Pose/

Defines pose data structures and handlers:
- `AvatarPoseData`: Complete avatar pose (positions and rotations)
- `PoseHandler`: Filters and stores pose data
- `IAvatarPoseFilter`: Interface for pose filtering

### FaceApplier/

Applies facial expressions to avatar models:
- `IFaceApplier`: Interface for face application
- `Vrm0xFaceApplier`: VRM 0.x blendshape application
- `Vrm10FaceApplier`: VRM 1.0 expression application

### Interpolation/

Handles frame rate differences between motion capture data (typically 30Hz) and application frame rate. Provides smooth animation by interpolating between keyframes:
- `PoseInterpolator`: Interpolates pose data (positions and rotations)
- `BlendshapeInterpolator<T>`: Interpolates blendshape weights

### Setup/

Initialization and calibration utilities for avatar animation:
- `VRIKSetup`: Initializes FinalIK VRIK component and IK targets
- `ArmIkSetup`: Initializes FinalIK ArmIK components
- `ArmLengthCalibrator`: Calibrates scale ratio between user and avatar based on arm length

### Utils/

General-purpose utilities:
- `BodyPartsBones`: Defines HumanBodyBones groups by body part (spine, arms, legs, fingers, eyes)
- `AssetsPositionAdjuster`: Adjusts SkinnedMeshRenderer bounds to prevent rendering issues

## Main Components

### AvatarAnimator

The main facade component that orchestrates the entire animation pipeline:
1. Receives landmarks from `LandmarkHandler`
2. Solves pose using `HolisticSolver`
3. Applies pose through VRIK or direct bone manipulation
4. Applies facial expressions through `IFaceApplier`

### AnimationResultData

Output data structure containing:
- `PoseData`: Avatar pose (positions and rotations)
- `VrmBlendshapes`: Standard VRM blendshape weights
- `PerfectSyncBlendshapes`: MediaPipe blendshape weights (for PerfectSync)

## Data Flow Details

1. **Input**: `LandmarkProvider` receives motion capture data
2. **Filtering**: `LandmarkHandler` filters and validates landmark data
3. **Solving**: `HolisticSolver` converts landmarks to `AvatarPoseData`
4. **Interpolation**: `PoseInterpolator` smooths pose data for higher frame rates
5. **Application**: `AvatarAnimator` applies pose to avatar via IK or direct rotation
6. **Face**: `IBlendshapeSolver` calculates blendshapes, `IFaceApplier` applies them
