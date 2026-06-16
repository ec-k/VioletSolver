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

### Solver 発火ごと：FK による avatarWristToDistal 計算

FK 計算は Unity の毎フレームではなく、`HolisticSolver.Solve` が呼ばれるたびに実行する。必要な入力（`handData.Wrist`、指関節回転）はすべて同一の `Solve` 呼び出し内で計算済みであるため、追加の遅延なく FK を適用できる。

HandSolver が当 Solve で計算した関節ローカル回転と、当 Solve 時点の手首ワールド回転から FK を計算する。

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

`wristWorldRot` は IK ターゲットのワールド回転であり、次のように計算する。

```csharp
Quaternion wristWorldRot = ikRigRoot.transform.rotation * handData.Wrist;
```

HandSolver が出力する `handData.Wrist` は ikRigRoot のローカル空間における回転であるため、ikRigRoot のワールド回転を乗算してワールド回転に変換する必要がある。ikRigRoot に offset が適用されていない場合は identity になるが、offset がある場合はずれるため常にこの形で計算すること。

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

# Solver 発火ごと（HolisticSolver.Solve 内）
R_wrist = ikRigRoot.rotation * handData.Wrist  (wrist world rotation)
R_mcp   = R_wrist * mcpLocalRot                (mcp world rotation)
R_pip   = R_mcp   * pipLocalRot                (pip world rotation)

O_avatar = R_wrist * L_mcp
         + R_mcp   * L_pip
         + R_pip   * L_dip

P_target_dip = P_wrist_pose + (P_dip_hand - P_wrist_hand)
P_ik_wrist   = P_target_dip - O_avatar
```

## 実装上の注意

### 計算タイミング

FK 計算は `HolisticSolver.Solve` 内で `HandSolver.SolveLeftHand` / `SolveRightHand` を呼んだ直後に実行する。Unity の毎フレーム（`Update`）ではなく、Solver が発火するたびに 1 回実行すれば十分。必要な入力はすべて同一 `Solve` 呼び出し内で揃うため、外部からの追加タイミング管理は不要。

### HandSolver からの回転取得

FK 計算で使う関節回転は、HandSolver が**当 Solve で計算した値**でなければならない。

- HandSolver が `Transform.localRotation` を**直接セット**する場合：`transform.localRotation` を読めば当 Solve の値が取れる
- **Animator パラメータ経由**でセットする場合：`Transform.localRotation` もアニメータ評価後にならないと反映されない。この場合は HandSolver が計算した回転値を内部フィールドとして公開し、それを直接参照する必要がある

### DIP ボーンの回転は不要

FK 計算では DIP 関節の回転（`dipLocalRot`）は使用しない。IndexDistal ボーンの**位置**（= DIP 関節の位置）を求めることが目的であるため、DIP ボーン自体の向きは関係しない。

### 手首回転の整合性

`wristWorldRot` は `ikRigRoot.transform.rotation * handData.Wrist` で計算したワールド回転を用いる。VRIK の IK ターゲットに設定する手首回転（`target.localRotation = handData.Wrist`）と FK 計算で用いる `wristWorldRot` は同一のワールド回転を表していなければならない。

## 関連ファイル

- `docs/ik-fingertip-alignment.md`: V1 のアルゴリズム
- `Assets/VioletSolver/Runtime/Solvers/FingertipAlignmentSolver.cs`: 指先アライメントソルバー
- `Assets/VioletSolver/Runtime/Solvers/HolisticSolver.cs`: ホリスティックソルバー
- `Assets/VioletSolver/Runtime/Solvers/HandSolver.cs`: ハンドソルバー
- `Assets/VioletSolver/Runtime/Setup/VRIKSetup.cs`: VRIK 初期化
