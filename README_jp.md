# Violet Solver

Violet Solver は，MediaPipe や Kinect などの姿勢推定結果を受け取り，VRM アバターのボーンや表情に反映する Unity パッケージです．VTuber 配信やモーションキャプチャなど，リアルタイムにアバターを動かす用途で使用できます．

## 主な機能

- MediaPipe と Kinect の姿勢推定データに対応
- UDPで受信した姿勢推定データを Unity のヒューマノイドのボーン構造にマッピング
- 表情は，VRMの標準的なBlendshapesと，ARKitのBlendshpesに対応

## 設計思想

本パッケージは，[Animaze](https://store.steampowered.com/app/1364390/Animaze_by_FaceRig) や [VSeeFace](https://www.vseeface.icu/)，[VRigUnity](https://github.com/Kariaro/VRigUnity) のようなアバタートラッキングアプリの開発を動機として生まれました．これらのアプリでは，姿勢推定とアバターへの反映という2つの処理が密結合になりがちですが，本パッケージはアバターへの反映部分のみを切り出しています．

姿勢推定の手法はデバイスや用途に応じて多様に存在し，今後も新しい手法が登場し続けます．一方で，対象はいずれも人体であるため，推定結果の骨格構造は手法が異なっても大きくは逸脱せず，各関節が位置・信頼度（場合によっては向き）を持つという点も概ね共通しています．そのため反映処理の大部分は使い回すことができ，推定手法をなるべく少ない変更で差し替えられる設計としています．

また，単体アプリではなくパッケージとして提供することで，VTuber 配信に限らず，ソーシャル VR など幅広い用途に組み込めるようにしています．

## セットアップ

Violet Solver を使用するには，以下の依存関係をプロジェクトに導入する必要があります．

### 1. Google.Protobuf のインストール

[NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity) を使用して，[Google.Protobuf](https://www.nuget.org/packages/google.protobuf/) パッケージをインストールします．

### 2. Final IK のインポート

[Final IK](https://assetstore.unity.com/packages/tools/animation/final-ik-14290) は，アバターの IK 処理に必要です．

1.  Unity のパッケージマネージャーから Final IK をプロジェクトにインポートします．
2.  インポート後，`Assets/Plugins/RootMotion/Import Assembly Definitions` を実行し，Final IK のアセンブリ定義ファイルをインポートしてください．

### 3. GitDependencyResolver のインポート

本パッケージが依存している OpenUPM で公開されていないパッケージを解決するため，[GitDependencyResolverForUnity](https://github.com/mob-sakai/GitDependencyResolverForUnity) をインポートしてください．

## UPM (Unity Package Manager) での導入

このパッケージは Unity Package Manager を通じて Git URL から直接インポートできます．

1.  Unity エディタを開きます．
2.  `Window` > `Package Manager` を選択します．
3.  左上の `+` ボタンをクリックし，`Add package from git URL...` を選択します．
4.  以下の URL を入力し，`Add` をクリックします．

    ```
    https://github.com/ec-k/VioletSolver.git?path=/Assets/VioletSolver#v0.2.1
    ```

これで Violet Solver パッケージがプロジェクトに導入されます．
