using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using uOSC;
using UnityEngine;

namespace VioletSolver.Network
{
    [RequireComponent(typeof(uOscServer))]
    public class PoseReceiver : MonoBehaviour
    {
        [SerializeField] uOscServer _server;
        public bool State { get; private set; } = false;

        public Dictionary<HumanBodyBones, Quaternion> Results { get; set; }

        void Start()
        {
            Results = new();
            _server.onDataReceived.AddListener(OnDataReceived);
        }

        void OnDataReceived(Message message)
        {
            if (message.address == "state")
            {
                State = (bool)message.values[0];
            }
            else
            {
                var boneKey = (HumanBodyBones)Enum.Parse(typeof(HumanBodyBones), message.address);
                var rotation = new Quaternion((float)message.values[0], (float)message.values[1], (float)message.values[2], (float)message.values[3]);
                Results[boneKey] = rotation;
            }
        }
    }
}
