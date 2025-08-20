using UnityEngine;

namespace VioletSolver
{
    public class Timer
    {
        public double CurrentTimeMs { get; private set; }
        public bool IsPlaying { get; private set; }
        public float PlaybackSpeed { get; set; } = 1.0f;

        public void Start()
        {
            IsPlaying = true;
        }

        public void Pause()
        {
            IsPlaying = false;
        }

        public void Update()
        {
            if (IsPlaying)
                CurrentTimeMs += Time.deltaTime * PlaybackSpeed * 1000.0f;
        }

        public void Reset()
        {
            CurrentTimeMs = 0;
            IsPlaying = false;
        }
    }
}
