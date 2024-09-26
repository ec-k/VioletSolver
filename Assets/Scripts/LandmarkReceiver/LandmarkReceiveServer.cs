using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using HolisticPose;

namespace VioletSolver.Network {
    public class LandmarkReceiveServer : MonoBehaviour
    {
        UdpClient _udpClient;
        [SerializeField] int _port = 9000;

        readonly Action<SocketException> _SocketEcxeptionCallback;
        readonly Action<ObjectDisposedException> _ObjectDisposedExceptionCallback;

        // Start is called before the first frame update
        void Start()
        {
            _udpClient = new UdpClient(_port);
            _udpClient.BeginReceive(OnReceived, _udpClient);
        }

        void OnReceived(IAsyncResult result)
        {
            UdpClient getUdp = (UdpClient)result.AsyncState;
            IPEndPoint ipEnd = null;

            try
            {
                byte[] getByte = getUdp.EndReceive(result, ref ipEnd);

                var message = HolisticLandmarks.Parser.ParseFrom(getByte);
                Debug.Log(message.PoseLandmarks.Landmark);
            }
            catch (SocketException e)
            {
                _SocketEcxeptionCallback(e);
                return;
            }catch (ObjectDisposedException e)
            {
                _ObjectDisposedExceptionCallback(e);
                return;
            }

            _udpClient.BeginReceive(OnReceived, getUdp);
        }

        public void Dispose()
        {
            _udpClient?.Dispose();
        }
    }
}