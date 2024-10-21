using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace VioletSolver.Network {
    public class LandmarkReceiver : MonoBehaviour
    {
        UdpClient _udpClient;
        [SerializeField] int _port = 9000;

        readonly Action<SocketException> _SocketEcxeptionCallback;
        readonly Action<ObjectDisposedException> _ObjectDisposedExceptionCallback;

        HolisticPose.HolisticLandmarks _results;
        public HolisticPose.HolisticLandmarks Results => _results;

        float _time = 0f;
        public float Time => _time;

        SynchronizationContext _context;

        // Start is called before the first frame update
        void Start()
        {
            _context = SynchronizationContext.Current;

            _results = new HolisticPose.HolisticLandmarks();
            _udpClient = new UdpClient(_port);
            _udpClient.BeginReceive(OnReceived, _udpClient);
            _time = UnityEngine.Time.time;
        }

        void OnReceived(IAsyncResult result)
        {
            UdpClient getUdp = (UdpClient)result.AsyncState;
            IPEndPoint ipEnd = null;

            try
            {
                byte[] getByte = getUdp.EndReceive(result, ref ipEnd);
                _results = HolisticPose.HolisticLandmarks.Parser.ParseFrom(getByte);
                _context.Post(_ =>
                {
                    _time = UnityEngine.Time.time;
                }, null);
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
            _udpClient?.Close();
            _udpClient?.Dispose();
        }

        public void OnApplicationQuit()
        {
            Dispose();
        }
    }
}