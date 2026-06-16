---
created: 2026-06-16
---
# IK ターゲット調整：指先アライメント手法 V2

## V1 の問題と改善動機

### V1 における一フレーム遅延

V1 では `avatarWristToDistal` を以下のように計算していた。

```csharp
Vector3 avatarWristToDistal = distalTransform.position - wristTransform.position;
```

`distalTransform.position` は `Animator.GetBoneTransform` で取得した Transform の位置を参照する。Unity の実行順において、HandSolver が当フレームの指回転をセットした後、アニメータ評価が走り、その結果がボーン Transform に反映される。そのため、アニメータ評価前に `SolveWristPosition` を呼んだ場合、`distalTransform.position` は前フレームのボーン位置となる。

### LPF による遅延との関係

一フレーム遅延を解消する方法として、`SolveWristPosition` の呼び出しタイミングをアニメータ評価後に移す案が考えられる。しかし、指関節の角度推定にはローパスフィルタ（LPF）がかかっており、データ自体に遅延が乗っている。タイミングの変更は Transform 読み取りの遅延を解消するに過ぎず、遅延の本質的な解決にはならない。

### 解決方針

ボーン Transform の位置読み取りを排除し、当フレームに HandSolver が計算した関節回転と、事前に取得した固定ボーンオフセットベクトルから、フォワードキネマティクス（FK）で `avatarWristToDistal` を算出する。

これにより、アニメータ評価タイミングへの依存がなくなる。残る遅延は LPF 由来のみとなる。

## アルゴリズム

### 基本方針

V1 のターゲット DIP 位置の計算はそのまま維持する。変更するのは `avatarWristToDistal` の算出方法のみ。

```
P_ik_wrist = P_target_dip - O_avatar
```

`O_avatar`（= `avatarWristToDistal`）を FK で求める。

### 初期化時：ボーンオフセットベクトルの取得

指ボーンのローカル位置はポーズに依らず固定である。初期化時に一度だけ取得して保持する。

```csharp
Vector3 localMcpOffset = indexProximalTransform.localPosition;     // wrist local
Vector3 localPipOffset = indexIntermediateTransform.localPosition;  // proximal local
Vector3 localDipOffset = indexDistalTransform.localPosition;        // intermediate local
```

> **注意**: スカラーの距離ではなく、各親ボーンのローカル座標系における**オフセットベクトル**が必要。スカラー距離だけでは指の向きが定まらない。

### 毎フレーム：FK による avatarWristToDistal 計算

HandSolver が当フレームに計算した関節ローカル回転と、当フレームの手首ワールド回転から FK を計算する。

```csharp
// Joint rotations calculated by HandSolver in the current frame
Quaternion mcpLocalRot = /* obtained from HandSolver */;
Quaternion pipLocalRot = /* obtained from HandSolver */;

// Accumulate world rotations along the chain
Quaternion mcpWorldRot = wristWorldRot * mcpLocalRot;
Quaternion pipWorldRot = mcpWorldRot   * pipLocalRot;

// Compute avatar wrist-to-DIP offset via FK
Vector3 avatarWristToDistal =
    wristWorldRot * localMcpOffset
  + mcpWorldRot   * localPipOffset
  + pipWorldRot   * localDipOffset;
```

`wristWorldRot` は MediaPipe.Hand から計算された当フレームの手首ワールド回転を使用する。

### IK 手首位置の計算（V1 と同じ）

```csharp
Vector3 targetIndexDip = kinectWristPos + (mpHandIndexDip - mpHandWrist);
Vector3 ikWristPosition = targetIndexDip - avatarWristToDistal;
```

## フォーミュラサマリ

```
# 初期化時（固定値）
L_mcp = IndexProximal.localPosition     (wrist local)
L_pip = IndexIntermediate.localPosition  (proximal local)
L_dip = IndexDistal.localPosition        (intermediate local)

# 毎フレーム
R_wrist = wrist world rotation          (from MediaPipe.Hand, current frame)
R_mcp   = R_wrist * mcpLocalRot         (mcp world rotation)
R_pip   = R_mcp   * pipLocalRot         (pip world rotation)

O_avatar = R_wrist * L_mcp
         + R_mcp   * L_pip
         + R_pip   * L_dip

P_target_dip = P_wrist_pose + (P_dip_hand - P_wrist_hand)
P_ik_wrist   = P_target_dip - O_avatar
```

## 実装上の注意

### HandSolver からの回転取得

FK 計算で使う関節回転は、HandSolver が**当フレームに計算した値**でなければならない。

- HandSolver が `Transform.localRotation` を**直接セット**する場合：`transform.localRotation` を読めば当フレームの値が取れる
- **Animator パラメータ経由**でセットする場合：`Transform.localRotation` もアニメータ評価後にならないと反映されない。この場合は HandSolver が計算した回転値を内部フィールドとして公開し、それを直接参照する必要がある

### DIP ボーンの回転は不要

FK 計算では DIP 関節の回転（`dipLocalRot`）は使用しない。IndexDistal ボーンの**位置**（= DIP 関節の位置）を求めることが目的であるため、DIP ボーン自体の向きは関係しない。

### 手首回転の整合性

`wristWorldRot` は MediaPipe.Hand から計算した当フレームの値を用いる。VRIK に渡す手首回転と FK 計算で用いる手首回転は同一の値を使うこと。

## 関連ファイル

- `docs/ik-fingertip-alignment.md`: V1 のアルゴリズム
- `Assets/VioletSolver/Runtime/Solvers/FingertipAlignmentSolver.cs`: 指先アライメントソルバー
- `Assets/VioletSolver/Runtime/Solvers/HolisticSolver.cs`: ホリスティックソルバー
- `Assets/VioletSolver/Runtime/Solvers/HandSolver.cs`: ハンドソルバー
- `Assets/VioletSolver/Runtime/Setup/VRIKSetup.cs`: VRIK 初期化
