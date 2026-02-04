# VioletSolver Runtime アーキテクチャ

このドキュメントでは、VioletSolver ランタイムのアーキテクチャとデータフローについて説明する。

## パイプライン概要

VioletSolver は、モーションキャプチャソースからランドマークデータを受け取り、ヒューマノイドアバターに適用する。データは以下のパイプラインを通過する：

```
LandmarkProvider -> Landmark -> Solver -> Pose -> AvatarAnimator
                                                        |
                                                  Interpolation
                                                  (フレームレート対応)
```

## ディレクトリ構造

```
Runtime/
├── LandmarkProviders/   # 入力源
├── Landmark/            # ランドマークデータ管理
├── Solvers/             # ランドマーク→ポーズ変換
├── Pose/                # ポーズデータ構造
├── FaceApplier/         # 顔表情適用
├── Interpolation/       # フレームレート対応
├── Setup/               # 初期化・キャリブレーション
├── Utils/               # 汎用ユーティリティ
├── AvatarAnimator.cs    # メインコンポーネント（ファサード）
└── AnimationResultData.cs
```

## 各ディレクトリの役割

### LandmarkProviders/

各種入力ソース（MediaPipe、Kinect等）からランドマークデータを提供する。`ILandmarkProvider` インターフェースを実装し、異なるモーションキャプチャシステムを抽象化する。

### Landmark/

生のランドマークデータを管理・処理する：
- `LandmarkHandler`: ランドマークデータのフィルタリングと管理
- `HolisticLandmarks`: 統合ランドマークデータ構造
- `LandmarksData`: 基本ランドマークデータ型

### Solvers/

ランドマーク位置をアバターのポーズ回転に変換する：
- `HolisticSolver`: ポーズ計算を統括するメインソルバー
- `MediaPipePoseSolver`: MediaPipeランドマーク用ソルバー
- `KinectPoseSolver`: Kinectランドマーク用ソルバー
- `HandSolver`: 指のポーズ計算
- `Face/`: 顔表情用ブレンドシェイプソルバー
- `RestPose/`: アバターのレストポーズデータ管理

### Pose/

ポーズデータ構造とハンドラを定義する：
- `AvatarPoseData`: 完全なアバターポーズ（位置と回転）
- `PoseHandler`: ポーズデータのフィルタリングと保持
- `IAvatarPoseFilter`: ポーズフィルタリング用インターフェース

### FaceApplier/

アバターモデルに顔表情を適用する：
- `IFaceApplier`: 顔表情適用のインターフェース
- `Vrm0xFaceApplier`: VRM 0.x ブレンドシェイプ適用
- `Vrm10FaceApplier`: VRM 1.0 表情適用

### Interpolation/

モーションキャプチャデータ（通常30Hz）とアプリケーションのフレームレートの差を吸収する。キーフレーム間を補間することで滑らかなアニメーションを実現する：
- `PoseInterpolator`: ポーズデータ（位置と回転）の補間
- `BlendshapeInterpolator<T>`: ブレンドシェイプウェイトの補間

### Setup/

アバターアニメーションの初期化とキャリブレーションユーティリティ：
- `VRIKSetup`: FinalIK VRIK コンポーネントとIKターゲットの初期化
- `ArmIkSetup`: FinalIK ArmIK コンポーネントの初期化
- `ArmLengthCalibrator`: 腕の長さに基づくユーザーとアバター間のスケール比率キャリブレーション

### Utils/

汎用ユーティリティ：
- `BodyPartsBones`: 体の部位別（脊椎、腕、脚、指、目）に HumanBodyBones をグループ化
- `AssetsPositionAdjuster`: レンダリング問題を防ぐため SkinnedMeshRenderer の bounds を調整

## 主要コンポーネント

### AvatarAnimator

アニメーションパイプライン全体を統括するメインファサードコンポーネント：
1. `LandmarkHandler` からランドマークを受け取る
2. `HolisticSolver` でポーズを計算する
3. VRIK または直接ボーン操作でポーズを適用する
4. `IFaceApplier` で顔表情を適用する

### AnimationResultData

出力データ構造：
- `PoseData`: アバターポーズ（位置と回転）
- `VrmBlendshapes`: 標準VRMブレンドシェイプウェイト
- `PerfectSyncBlendshapes`: MediaPipeブレンドシェイプウェイト（PerfectSync用）

## データフロー詳細

1. **入力**: `LandmarkProvider` がモーションキャプチャデータを受信
2. **フィルタリング**: `LandmarkHandler` がランドマークデータを検証・フィルタリング
3. **ソルビング**: `HolisticSolver` がランドマークを `AvatarPoseData` に変換
4. **補間**: `PoseInterpolator` が高フレームレート向けにポーズデータを平滑化
5. **適用**: `AvatarAnimator` がIKまたは直接回転でアバターにポーズを適用
6. **顔**: `IBlendshapeSolver` がブレンドシェイプを計算し、`IFaceApplier` が適用
