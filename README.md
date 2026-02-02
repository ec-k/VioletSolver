# Violet Solver

Violet Solver is a Unity package that takes pose estimation results from sources such as MediaPipe and Kinect and applies them to VRM avatar bones and facial expressions. It is designed for real-time avatar animation use cases such as VTuber streaming and motion capture.

## Features

- Supports pose estimation data from MediaPipe and Kinect
- Maps pose estimation data received via UDP to Unity Humanoid bone structure
- Supports VRM standard Blendshapes and ARKit Blendshapes for facial expressions

## Design Philosophy

This package was inspired by avatar tracking applications such as [Animaze](https://store.steampowered.com/app/1364390/Animaze_by_FaceRig), [VSeeFace](https://www.vseeface.icu/), and [VRigUnity](https://github.com/Kariaro/VRigUnity). In these applications, pose estimation and avatar reflection tend to be tightly coupled. This package extracts only the avatar reflection part.

There are many pose estimation methods depending on the device and use case, and new methods will continue to emerge. However, since they all target the human body, the skeletal structures of estimation results do not vary drastically across methods, and joints generally share common attributes such as position, confidence, and sometimes orientation. This means that most of the avatar reflection logic can be reused, and the package is designed so that the estimation method can be swapped with minimal changes.

By providing this as a package rather than a standalone application, it can be integrated into a wide range of use cases beyond VTuber streaming, such as social VR applications.

## Setup

To use Violet Solver, you need to install the following dependencies in your project.

### 1. Install Google.Protobuf

Install the [Google.Protobuf](https://www.nuget.org/packages/google.protobuf/) package using [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity).

### 2. Import Final IK

[Final IK](https://assetstore.unity.com/packages/tools/animation/final-ik-14290) is required for avatar IK processing.

1. Import Final IK into your project from the Unity Package Manager.
2. After importing, run `Assets/Plugins/RootMotion/Import Assembly Definitions` to import the Final IK assembly definition files.

### 3. Import GitDependencyResolver

Import [GitDependencyResolverForUnity](https://github.com/mob-sakai/GitDependencyResolverForUnity) to resolve packages that this package depends on but are not published on OpenUPM.

## Installation via UPM (Unity Package Manager)

This package can be imported directly from a Git URL through the Unity Package Manager.

1. Open the Unity Editor.
2. Select `Window` > `Package Manager`.
3. Click the `+` button in the top left and select `Add package from git URL...`.
4. Enter the following URL and click `Add`.

    ```
    https://github.com/ec-k/VioletSolver.git?path=/Assets/VioletSolver#v0.2.1
    ```

This will install the Violet Solver package in your project.
