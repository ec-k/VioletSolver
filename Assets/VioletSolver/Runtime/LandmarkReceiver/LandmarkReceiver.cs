using UnityEngine;
using System;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace VioletSolver.Network 
{
    internal class LandmarkReceiver : MonoBehaviour
    {
        UdpClient _udpClient;
        [SerializeField] int _port = 9000;

        readonly Action<SocketException> _SocketEcxeptionCallback;
        readonly Action<ObjectDisposedException> _ObjectDisposedExceptionCallback;

        HolisticPose.HolisticLandmarks _results;
        internal HolisticPose.HolisticLandmarks Results => _results;

        float _time = 0f;
        internal float Time => _time;

        // Start is called before the first frame update
        void Start()
        {
            _time = UnityEngine.Time.time;
            _results = new HolisticPose.HolisticLandmarks();
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
                    _results = HolisticPose.HolisticLandmarks.Parser.ParseFrom(getByte);
                    await UniTask.SwitchToMainThread();
                    _time = UnityEngine.Time.time;  // NOTE: This process works only main thread.
                    await UniTask.SwitchToThreadPool();
                }
                catch (SocketException e)
                {
                    _SocketEcxeptionCallback(e);
                    return;
                }
                catch (ObjectDisposedException e)
                {
                    _ObjectDisposedExceptionCallback(e);
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