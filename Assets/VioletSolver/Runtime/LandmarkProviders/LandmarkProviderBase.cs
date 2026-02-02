using System;
using UnityEngine;
using VioletSolver.LandmarkProviders;

namespace VioletSolver
{
    public abstract class LandmarkProviderBase : MonoBehaviour, ILandmarkProvider
    {
        public abstract event Action<HumanLandmarks.HolisticLandmarks, float> OnLandmarksReceived;
    }
}
