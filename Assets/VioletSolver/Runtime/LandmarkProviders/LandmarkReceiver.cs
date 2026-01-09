using UnityEngine;
using System;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Net;

namespace VioletSolver.LandmarkProviders 
{
    public class LandmarkReceiver : MonoBehaviour, ILandmarkProvider, IDisposable
    {
        UdpClient _udpClient;
        bool _isDisposed = false;
        [SerializeField] int _port = 9000;

        public event Action<HumanLandmarks.HolisticLandmarks, float> OnLandmarksReceived;

        void Start()
        {
            var ipEnd = new IPEndPoint(System.Net.IPAddress.Any, _port);
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
                        await UniTask.SwitchToMainThread();
                        var receivedTime = UnityEngine.Time.time;  // NOTE: This process works only main thread.
                        OnLandmarksReceived?.Invoke(receivedLandmarks, receivedTime);
                        await UniTask.SwitchToThreadPool();
                    }
                    catch (Exception e) when (e is SocketException or ObjectDisposedException)
                    {
                        Debug.LogError($"Exception: {e.Message}");
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
