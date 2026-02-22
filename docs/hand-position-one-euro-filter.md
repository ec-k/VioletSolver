---
created: 2026-02-23
---
# Hand Position One Euro Filter

This document explains the background and design decisions behind `HandPositionOneEuroFilter`.

## Background

VioletSolver uses `FingertipAlignmentSolver` to calculate wrist positions in IK mode. This solver adjusts the IK wrist position so that the avatar's index finger distal bone aligns with the MediaPipe.Hand index finger DIP.

See `docs/ik-fingertip-alignment.md` for details.

## Problem: Hand Jitter

When `FingertipAlignmentSolver` is applied directly, the avatar's hands shake (jitter occurs).

## Cause Analysis

### Combining Multiple Data Sources

The IK wrist position calculation involves three different data sources:

```
ikWristPosition = (poseWristPosition + userWristToDip) - avatarWristToDistal
```

| Data | Source | Notes |
|------|--------|-------|
| `poseWristPosition` | Kinect/Pose | Wrist position for current frame |
| `userWristToDip` | MediaPipe.Hand | Wrist to DIP offset for current frame |
| `avatarWristToDistal` | Avatar bones | Wrist to distal vector from **previous frame** |

### Phase and Latency Mismatch

Each data source has different latency characteristics:

#### Latency from System Architecture

Kinect and MediaPipe each run inference in separate processes. Images are passed via shared memory, and estimation results are sent to this application via UDP. This architecture causes phase shifts due to processing time and network latency in each process.

#### Latency from Different LPF Strengths

Kinect and MediaPipe differ significantly in accuracy and stability, so different LPF (low-pass filter) strengths are applied to each. Stronger LPF results in greater latency, making this a major factor in the phase difference between the two.

#### Avatar Bone 1-Frame Delay

`animator.GetBoneTransform()` returns the state after IK was applied in the previous frame.

---

Because these three data sources are updated with different latencies, the combined result has per-frame inconsistencies.

### Feedback Loop

`avatarWristToDistal` depends on the previous frame's IK result:

```
Frame N ikWristPosition
    ↓ (IK applied)
Frame N avatar bone state
    ↓ (referenced in next frame)
Frame N+1 avatarWristToDistal
    ↓ (used in calculation)
Frame N+1 ikWristPosition
```

This feedback loop creates a structure where noise is easily amplified.

## Solution: One Euro Filter

### Why Apply Filter to Avatar Bones Rather Than Landmarks

The root cause of jitter is timing mismatch (phase shift). Even if LPF is applied individually to each landmark, their phases remain misaligned, so the instability in the combined result is not resolved.

Therefore, the filter must be applied to the final combined result: the IK wrist position.

## Implementation

### Filter Application Point

```
HolisticSolver.Solve()
    └─ FingertipAlignmentSolver.SolveWristPosition()
           ↓
       AvatarPoseData (LeftHandPosition, RightHandPosition)
           ↓
PoseHandler.Update()
    └─ HandPositionOneEuroFilter.Filter()  ← Filtering applied here
           ↓
       Filtered AvatarPoseData
           ↓
AvatarAnimator.ApplyIkTarget()
    └─ Applied to VRIK
```

### Parameters

```csharp
public HandPositionOneEuroFilter(
    float minCutoff = 1.0f,   // Minimum cutoff frequency (lower = smoother when stationary)
    float beta = 0.007f,      // Speed coefficient (higher = more responsive when moving)
    float dCutoff = 1.0f      // Cutoff frequency for velocity smoothing
)
```

## Related Files

- `Assets/VioletSolver/Runtime/Pose/HandPositionOneEuroFilter.cs`: One Euro Filter implementation
- `Assets/VioletSolver/Runtime/Solvers/FingertipAlignmentSolver.cs`: Fingertip alignment solver
- `Assets/VioletSolver/Runtime/Solvers/HolisticSolver.cs`: Holistic solver
- `Assets/VioletSolver/Runtime/Pose/PoseHandler.cs`: Pose filter application
- `Assets/VioletSolver/Runtime/AvatarAnimator.cs`: Animation application

## References

- [1€ Filter](https://gery.casiez.net/1euro/) - One Euro Filter original paper
