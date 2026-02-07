# Violet Solver

Violet Solver は、MediaPipe や Kinect といった外部ソースからの姿勢推定結果を Unity アバターの姿勢に変換するためのパッケージです。リアルタイムでのアバター制御や、モーションキャプチャデータの活用を目的としています。

## 特徴

- MediaPipe および Kinect の姿勢推定データに対応
- 取得した姿勢データを Unity アバターのボーン構造にマッピング
- リアルタイムでのアバターアニメーション生成

## セットアップ

Violet Solver を使用するには、以下の依存関係をプロジェクトに導入する必要があります。

### 1. Google.Protobuf のインストール

[NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity) を使用して、[Google.Protobuf](https://www.nuget.org/packages/google.protobuf/) パッケージをインストールします。

### 2. Final IK のインポート

[Final IK](https://assetstore.unity.com/packages/tools/animation/final-ik-14290) は、アバターの IK 処理に必要です。

1.  Unity のパッケージマネージャーから Final IK をプロジェクトにインポートします。
2.  インポート後、`Assets/Plugins/RootMotion/Import Assembly Definitions` を実行し、Final IK のアセンブリ定義ファイルをインポートしてください。

### 3. GitDependencyResolver のインポート

本パッケージが依存している OpenUPM で公開されていないパッケージを解決するため，[GitDependencyResolverForUnity](https://github.com/mob-sakai/GitDependencyResolverForUnity) をインポートしてください．

## UPM (Unity Package Manager) での導入

このパッケージは Unity Package Manager を通じて Git URL から直接インポートできます。

1.  Unity エディタを開きます。
2.  `Window` > `Package Manager` を選択します。
3.  左上の `+` ボタンをクリックし、`Add package from git URL...` を選択します。
4.  以下の URL を入力し、`Add` をクリックします。

    ```
    https://github.com/ec-k/VioletSolver.git?path=/Assets/VioletSolver#release/v0.3.1
    ```

これで Violet Solver パッケージがプロジェクトに導入されます。
