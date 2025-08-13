using System;

namespace VioletSolver.LandmarkProviders
{
    public interface ILandmarkProvider
    {
        public event Action<HumanLandmarks.HolisticLandmarks, float> OnLandmarksReceived;
    }
}
