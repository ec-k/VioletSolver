using UnityEngine;
using System;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Net;
using System.Diagnostics;

namespace VioletSolver.LandmarkProviders 
{
    public class LandmarkReceiver : LandmarkProviderBase, IDisposable
    {
        UdpClient _udpClient;
        bool _isDisposed = false;
        [SerializeField] int _port = 9000;

        public override event Action<HumanLandmarks.HolisticLandmarks, float> OnLandmarksReceived;

        void Start()
        {
            var ipEnd = new IPEndPoint(IPAddress.Any, _port);
            _udpClient = new UdpClient(ipEnd);
            var token = this.GetCancellationTokenOnDestroy();
            OnReceived(token).Forget();
        }

        async UniTask<byte[]> ReceiveByte(CancellationToken token)
        {
            var result = await _udpClient.ReceiveAsync().AsUniTask(useCurrentSynchronizationContext: false).AttachExternalCancellation(token);
            var data = result.Buffer;
            return data;
        }

        async UniTaskVoid OnReceived(CancellationToken token)
        {
            try
            {
                await UniTask.SwitchToThreadPool();
                while (!token.IsCancellationRequested && !_isDisposed)
                {
                    try
                    {
                        byte[] getByte = await ReceiveByte(token);
                        var receivedLandmarks = HumanLandmarks.HolisticLandmarks.Parser.ParseFrom(getByte);
                        var receivedTimeSec = (float)Stopwatch.GetTimestamp() / Stopwatch.Frequency;
                        OnLandmarksReceived?.Invoke(receivedLandmarks, receivedTimeSec);
                    }
                    catch (Exception e) when (e is SocketException or ObjectDisposedException)
                    {
                        UnityEngine.Debug.LogError($"Exception: {e.Message}");
                        return;
                    }
                }
            }
            finally
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return; 

            _udpClient?.Close();
            _udpClient?.Dispose();
            _udpClient = null;

            _isDisposed = true;
        }

        void OnDestroy()
        {
            Dispose();
        }
    }
}
