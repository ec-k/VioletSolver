---
created: 2026-02-23
---
# Hand Position One Euro Filter

本文書では，`HandPositionOneEuroFilter`が必要となった背景と，その設計判断について説明する．

## 背景

VioletSolverでは，IKモードにおいて手首位置を計算する際に`FingertipAlignmentSolver`を使用している．このソルバーは，アバターの人差し指の末節骨（Distal）をMediaPipe.Handの人差し指DIPに合わせるようにIK手首位置を調整する．

詳細は `docs/ik-fingertip-alignment.ja.md` を参照．

## 問題：手のジッター

`FingertipAlignmentSolver`をそのまま適用すると，アバターの手がガタガタと震える（ジッターが発生する）．

## 原因分析

### 複数データソースの合成

IK手首位置の計算には，3つの異なるデータソースが関与している：

```
ikWristPosition = (poseWristPosition + userWristToDip) - avatarWristToDistal
```

| データ | ソース | 備考 |
|--------|--------|------|
| `poseWristPosition` | Kinect/Pose | 現在フレームの手首位置 |
| `userWristToDip` | MediaPipe.Hand | 現在フレームの手首→DIPオフセット |
| `avatarWristToDistal` | アバターボーン | **1フレーム前**の手首→Distalベクトル |

### 位相と遅延の不整合

各データソースには異なる遅延特性がある：

#### システム構成による遅延

KinectとMediaPipeはそれぞれ独立したプロセスで推論を行っている．画像は共有メモリ経由で渡され，推定結果はUDP経由で本アプリに送信される．この構成により，各プロセスの処理時間やネットワーク遅延の分だけ位相がずれる．

#### LPFの強さの違いによる遅延

KinectとMediaPipeは精度・安定性が大きく異なるため，それぞれに適用しているLPF（ローパスフィルター）の強さが異なる．LPFが強いほど遅延が大きくなるため，これが両者の位相差を生む主要因となっている．

#### アバターボーンの1フレーム遅延

`animator.GetBoneTransform()`は前フレームでIKが適用された後の状態を返す．

---

これら3つのデータは異なる遅延で更新されるため，合成結果にフレームごとの不整合が生じる．

### フィードバックループ

`avatarWristToDistal`は前フレームのIK結果に依存している．つまり：

```
Frame N の ikWristPosition
    ↓ (IK適用)
Frame N のアバターボーン状態
    ↓ (次フレームで参照)
Frame N+1 の avatarWristToDistal
    ↓ (計算に使用)
Frame N+1 の ikWristPosition
```

このフィードバックループにより，ノイズが増幅されやすい構造になっている．

## 解決策：One Euro Filter

### なぜランドマークではなくアバターボーンにフィルターを適用するか

ジッターの根本原因はタイミングの不整合（位相のずれ）である．各ランドマークに個別にLPFを適用しても，それぞれの位相は依然としてずれたままであり，合成結果の不安定さは解消されない．

したがって，最終的な合成結果であるIK手首位置にフィルターを適用するしかない．

## 実装

### フィルターの適用位置

```
HolisticSolver.Solve()
    └─ FingertipAlignmentSolver.SolveWristPosition()
           ↓
       AvatarPoseData (LeftHandPosition, RightHandPosition)
           ↓
PoseHandler.Update()
    └─ HandPositionOneEuroFilter.Filter()  ← ここでフィルタリング
           ↓
       フィルタ適用済み AvatarPoseData
           ↓
AvatarAnimator.ApplyIkTarget()
    └─ VRIKに適用
```

### パラメータ

```csharp
public HandPositionOneEuroFilter(
    float minCutoff = 1.0f,   // 最小カットオフ周波数（低いほど静止時に滑らか）
    float beta = 0.007f,      // 速度係数（高いほど動作時に追従）
    float dCutoff = 1.0f      // 速度平滑化のカットオフ周波数
)
```

## 関連ファイル

- `Assets/VioletSolver/Runtime/Pose/HandPositionOneEuroFilter.cs`: One Euro Filterの実装
- `Assets/VioletSolver/Runtime/Solvers/FingertipAlignmentSolver.cs`: 指先位置合わせソルバー
- `Assets/VioletSolver/Runtime/Solvers/HolisticSolver.cs`: 統合ソルバー
- `Assets/VioletSolver/Runtime/Pose/PoseHandler.cs`: ポーズフィルタの適用
- `Assets/VioletSolver/Runtime/AvatarAnimator.cs`: アニメーション適用

## 参考

- [1€ Filter](https://gery.casiez.net/1euro/) - One Euro Filterの原論文
