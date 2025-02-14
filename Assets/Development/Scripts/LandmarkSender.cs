using Google.Protobuf;
using System.Net.Sockets;
using UnityEngine;

namespace VioletSolver.Development
{
    public class LandmarkSender : MonoBehaviour
    {
        [SerializeField] AvatarAnimator _avatarAnimator;

        LandmarkHandler _landamrkHandler;
        UdpClient _sender;
        [SerializeField] int _port = 9002;

        [SerializeField] bool _isEnabled = false;

        void Start()
        {
            _landamrkHandler = _avatarAnimator.Landmarks;
            _sender = new UdpClient(_port);
            _sender.Connect("localhost", _port);
        }

        void Update()
        {
            if (_isEnabled)
                Send();
        }

        void Send()
        {
            if (_landamrkHandler.Landmarks == null)
                return;
            var data = Packer.Convert(_landamrkHandler.Landmarks);
            var message = data.ToByteArray();
            _sender.Send(message, message.Length);
        }
    }
}
