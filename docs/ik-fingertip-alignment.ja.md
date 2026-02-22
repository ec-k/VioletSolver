---
created: 2026-02-22T15:34
updated: 2026-02-22T15:43
---
# IKターゲット調整：指先位置合わせ手法

本文書では，セルフタッチ時に人差し指の先を正しく合わせるためのIKターゲット調整手法について説明する．

## 背景と課題

### 問題

アバターとユーザの体格差により，セルフタッチが正しく反映されない．例えば，ユーザが自分の顔を触る動作をしても，アバターの手が顔に届かない．

具体的には，首から顔の前面までの長さや腕に対する手の長さの比率が，アバターの方が実世界の人間よりも小さいことによる．

この文書に記述する方法は，特に，手の長さの差異を吸収するためのものである．

### 制約

FinalIK の VRIK は**手首の位置・回転**のみを設定できる．指先の位置を直接指定することはできない．

### 目標

MediaPipe.Hand で取得した人差し指の先の位置に，アバターの人差し指の先が正しく合うようにする．

## 入力データ

### Kinect（ポーズ）

- 手首の位置・回転を提供
- 全身のポーズ推定に使用

### MediaPipe.Hand

- 21点の手のランドマークを提供
- index 0: 手首
- index 8: 人差し指の先（IndexFingerTip）

## 手法

### 基本的な考え方

VRIKは手首しか制御できないため，「人差し指の先が目標位置に来るような手首位置」を逆算する．

ただし，KinectのHandTipは推定結果の精度が安定しないため，より信頼のおけるMediaPipe.Hand の結果を利用する．

### 実装手順

#### Step 1: 座標系の統合

Kinectの手首位置にMediaPipe.Handの手首（index 0）を合わせる．これにより，両センサーの座標系を統合する．

```
handWristWorld = kinectWristPosition
```

#### Step 2: 手首の回転を考慮したオフセット計算

アバターの「手首から人差し指先までのオフセット」を，現在の手首の回転を考慮して計算する．

```csharp
// アバターのローカル空間でのオフセット（バインドポーズまたは現在のポーズ）
Vector3 localOffset = avatarIndexTip - avatarWrist;  // ローカル空間

// 現在の手首回転を適用してワールド空間に変換
Vector3 rotatedOffset = wristRotation * localOffset;
```

#### Step 3: IK手首位置の計算

MediaPipe.Handの人差し指の先の位置から，回転済みオフセットを引いて手首位置を求める．

```csharp
// MediaPipe.Handの人差し指先（ワールド座標に変換済み）
Vector3 targetIndexTip = TransformToWorld(mediaPipeHand.Landmarks[8]);

// VRIKに渡す手首位置
Vector3 ikWristPosition = targetIndexTip - rotatedOffset;
```

#### Step 4: VRIKへの適用

計算した手首位置と，MediaPipe.Hand から導出した手首回転を VRIK に渡す．

```csharp
vrik.solver.leftArm.target.position = ikWristPosition;
vrik.solver.leftArm.target.rotation = wristRotation;
```

## 注意点

### IK適用前の値を使用する

`avatarIndexTip - avatarWrist` の計算には，IK適用前のボーン位置を使用すること．IK適用後の値を使うとフィードバックループが発生する．

対策：
- VRIKの `OnPreUpdate` で計算する
- または，IK適用前のボーン位置をキャッシュしておく

### 手首回転の整合性

手首の回転も正しく設定しないと，人差し指の先が目標位置に来ない．MediaPipe.Hand から手首の回転を計算して VRIK に渡す必要がある．

### スケール補正

アバターとユーザの体格差がある場合，ローカルオフセットにスケール補正が必要になる可能性がある．

```csharp
Vector3 scaledLocalOffset = localOffset * avatarScale;
```

## 数式まとめ

```
P_target    = MediaPipe.Hand 人差し指先のワールド位置
R_wrist     = 手首の回転（MediaPipe.Hand または Kinect から）
O_local     = アバターの手首→人差し指先のローカルオフセット
O_rotated   = R_wrist * O_local

P_ikWrist   = P_target - O_rotated
```

## 関連ファイル

- `Assets/VioletSolver/Runtime/Solvers/KinectPoseSolver.cs`: Kinectポーズソルバー
- `Assets/VioletSolver/Runtime/Solvers/HandSolver.cs`: 手のソルバー
- `Assets/VioletSolver/Runtime/Setup/VRIKSetup.cs`: VRIK初期化

### References
この手法は，以下の記事を参考にした．
- [Inverse Kinematics(IK)について, 2ボーンIKをキャラクターに適用する, エフェクターの目標位置の置き換え](https://techblog.sega.jp/entry/sega_inverse_kinematics202210)