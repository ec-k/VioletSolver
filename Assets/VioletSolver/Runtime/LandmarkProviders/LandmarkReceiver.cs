using UnityEngine;
using System;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace VioletSolver.LandmarkProviders 
{
    public class LandmarkReceiver : MonoBehaviour, ILandmarkProvider
    {
        UdpClient _udpClient;
        [SerializeField] int _port = 9000;

        public event Action<HumanLandmarks.HolisticLandmarks, float> OnLandmarksReceived;

        void Start()
        {
            _udpClient = new UdpClient(_port);
            var token = this.GetCancellationTokenOnDestroy();
            OnReceived(token).Forget();
        }

        async UniTask<byte[]> ReceiveByte(CancellationToken token)
        {
            var result = await _udpClient.ReceiveAsync();
            var data = result.Buffer;
            return data;
        }

        async UniTaskVoid OnReceived(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await UniTask.SwitchToThreadPool();
            while (!token.IsCancellationRequested)
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
                catch (SocketException e)
                {
                    Debug.LogError($"SocketException: {e.Message}");
                    return;
                }
                catch (ObjectDisposedException e)
                {
                    Debug.LogWarning($"ObjectDisposedException: {e.Message}");
                    return;
                }
            }
            token.ThrowIfCancellationRequested();
        }

        void Dispose()
        {
            _udpClient?.Close();
            _udpClient?.Dispose();
        }

        void OnApplicationQuit()
        {
            Dispose();
        }
    }
}