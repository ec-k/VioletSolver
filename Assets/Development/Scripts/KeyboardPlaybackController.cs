using Codice.Client.Common.Logging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VioletSolver.LandmarkProviders;

namespace VioletSolver.Development
{
    public class KeyboardPlaybackController : MonoBehaviour
    {
        [SerializeField] LandmarkPlaybackManager _playbackManager;
        bool IsPlayingLog => _playbackManager.IsPlayingLog;

        void Start()
        {
            if(_playbackManager == null)
            {
                Debug.LogError("Playback Manager is not assigned in the inspector.");
                enabled = false;
                return;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (IsPlayingLog)
                {
                    _playbackManager.PausePlayback();
                }
                else
                {
                    _playbackManager.StartPlayback();
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                _playbackManager.ResetPlayback();
            }
        }
    }
}
