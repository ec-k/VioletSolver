using System;
using Cysharp.Threading.Tasks;

namespace VioletSolver.LandmarkProviders
{
    public interface ILandmarkLogReader: IDisposable
    {
        event Action<HumanLandmarks.HolisticLandmarks, float> OnLandmarksReceived;
        event Action OnReachedToEnd;
        float PlaybackSpeed { get; set; }
        bool IsPlaying { get; }
        HumanLandmarks.Log.LogHeader LogHeader { get; }

        bool Initialize(string logFilePath);
        void StartPlayback();
        void PausePlayback();
        void ResetPlayback();
        UniTask Update();
    }
}
