---
created: 2026-02-22T15:34
updated: 2026-02-23T12:00
---
# IKターゲット調整：指先位置合わせ手法

本文書では，セルフタッチ時に人差し指を正しく合わせるためのIKターゲット調整手法について説明する．

## 背景と課題

### 問題

アバターとユーザの体格差により，セルフタッチが正しく反映されない．例えば，ユーザが自分の顔を触る動作をしても，アバターの手が顔に届かない．

具体的には，首から顔の前面までの長さや腕に対する手の長さの比率が，アバターの方が実世界の人間よりも小さいことによる．

この文書に記述する方法は，特に，手の長さの差異を吸収するためのものである．

### 制約

- FinalIK の VRIK は**手首の位置・回転**のみを設定できる．指先の位置を直接指定することはできない．
- Unity Humanoid には Tip（指先）ボーンが存在しない．Distal（末節骨）が最も先端のボーンである．

### 目標

MediaPipe.Hand で取得した人差し指の DIP 位置に，アバターの人差し指の Distal が正しく合うようにする．

## 入力データ

### Kinect（ポーズ）

- 手首の位置・回転を提供
- 全身のポーズ推定に使用

### MediaPipe.Hand

- 21点の手のランドマークを提供
- index 0: 手首（Wrist）
- index 7: 人差し指の DIP（IndexFingerDip）

## 手法

### 基本的な考え方

VRIKは手首しか制御できないため，「人差し指の Distal が目標位置に来るような手首位置」を逆算する．

KinectのHandTipは推定結果の精度が安定しないため，より信頼のおけるMediaPipe.Hand の結果を利用する．

Unity Humanoid には Tip ボーンがないため，MediaPipe.Hand 側も Tip ではなく DIP を使用し，アバター側の Distal と対応させる．

### 実装手順

#### Step 1: 座標系の統合

Kinectの手首位置にMediaPipe.Handの手首（index 0）を合わせる．これにより，両センサーの座標系を統合する．

```
handWristWorld = kinectWristPosition
```

#### Step 2: ターゲット DIP 位置の計算

MediaPipe.Hand の手首→DIP オフセットを Kinect の手首位置に加算し，ターゲット DIP 位置を求める．

```csharp
// MediaPipe.Hand の手首→DIP オフセット
Vector3 userWristToDip = handIndexDip - handWrist;

// ターゲット DIP 位置（ワールド座標）
Vector3 targetIndexDip = poseWristPosition + userWristToDip;
```

#### Step 3: アバターの手首→Distal ベクトルの取得

アバターの現在の手首から Distal までのベクトルをワールド座標で取得する．

```csharp
// アバターの現在のワールド座標での手首→Distal ベクトル
Vector3 avatarWristToDistal = distalTransform.position - wristTransform.position;
```

注意：この値は1フレーム前の指の状態に基づく．指の動きは比較的ゆっくりなので，実用上問題ない．

#### Step 4: IK手首位置の計算

ターゲット DIP 位置から，アバターの手首→Distal ベクトルを引いて手首位置を求める．

```csharp
// VRIKに渡す手首位置
Vector3 ikWristPosition = targetIndexDip - avatarWristToDistal;
```

#### Step 5: VRIKへの適用

計算した手首位置と，MediaPipe.Hand から導出した手首回転を VRIK に渡す．

```csharp
vrik.solver.leftArm.target.position = ikWristPosition;
vrik.solver.leftArm.target.rotation = wristRotation;
```

## 注意点

### 1フレーム遅延

`avatarWristToDistal` は `animator.GetBoneTransform` で取得するため，現在のフレームで計算した指の回転が適用される前の状態（1フレーム前）を使用する．指の動きは比較的ゆっくりなので，実用上は問題ない．

### 手首回転の整合性

手首の回転も正しく設定しないと，人差し指が目標位置に来ない．MediaPipe.Hand から手首の回転を計算して VRIK に渡す必要がある．

## 数式まとめ

```
P_wrist_pose     = Kinect/Pose の手首ワールド位置
P_dip_hand       = MediaPipe.Hand の DIP 位置（手首からの相対）
P_wrist_hand     = MediaPipe.Hand の手首位置（= 原点）

O_user           = P_dip_hand - P_wrist_hand  （ユーザの手首→DIP オフセット）
P_target_dip     = P_wrist_pose + O_user      （ターゲット DIP 位置）

O_avatar         = アバターの手首→Distal ベクトル（ワールド座標，1フレーム前）

P_ik_wrist       = P_target_dip - O_avatar
```

## 関連ファイル

- `Assets/VioletSolver/Runtime/Solvers/FingertipAlignmentSolver.cs`: 指先位置合わせソルバー
- `Assets/VioletSolver/Runtime/Solvers/HolisticSolver.cs`: 統合ソルバー
- `Assets/VioletSolver/Runtime/Solvers/HandSolver.cs`: 手のソルバー
- `Assets/VioletSolver/Runtime/Setup/VRIKSetup.cs`: VRIK初期化

### References
この手法は，以下の記事を参考にした．
- [Inverse Kinematics(IK)について, 2ボーンIKをキャラクターに適用する, エフェクターの目標位置の置き換え](https://techblog.sega.jp/entry/sega_inverse_kinematics202210)