using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using HumanLandmarks.Log;

namespace VioletSolver.LandmarkProviders
{
    /// <summary>
    /// Protobuf�o�C�i�����O�t�@�C����ǂݍ��݁A�A�o�^�[�p���f�[�^��񋟂��郊�[�_�[�B
    /// ILandmarkLogReader �C���^�[�t�F�[�X���������AUniTask ���g�p���Ĕ񓯊�I/O����������B
    /// </summary>
    public class LandmarkProtoBinaryLogReader : ILandmarkLogReader
    {
        FileStream _fileStream;
        BinaryReader _binaryReader;
        string _logFilePath;
        bool _isInitialized = false;
        long _nextFrameOffset = 0;
        double? _firstTimestampMillis = null;

        LogFrameData _currentFrameDataBuffer;

        bool _isPlaying = false;

        MessageParser<LogHeader> _logHeaderParser = new MessageParser<LogHeader>(() => new LogHeader());
        MessageParser<LogFrameData> _logFrameDataParser = new MessageParser<LogFrameData>(() => new LogFrameData());

        public event Action<HumanLandmarks.HolisticLandmarks, float> OnLandmarksReceived;

        public float PlaybackSpeed { get; set; } = 1.0f;
        public bool IsPlaying => _isPlaying;
        public LogHeader LogHeader { get; private set; }

        /// <summary>
        /// ���O�t�@�C�������������A�w�b�_��ǂݍ��ށB
        /// �C���^�[�t�F�[�X�̐���ɂ�蓯�����\�b�h�Ƃ��Ē񋟂��邪�A�����Ńt�@�C�����J���B
        /// </summary>
        /// <param name="logFilePath">���O�t�@�C���̃p�X�B</param>
        /// <returns>�����������������ꍇ�� true�A����ȊO�� false�B</returns>
        public bool Initialize(string logFilePath)
        {
            if (_isInitialized)
            {
                UnityEngine.Debug.LogWarning("LandmarkProtoBinaryLogReader �͊��ɏ���������Ă��܂��B");
                return true;
            }

            _logFilePath = Path.Combine(UnityEngine.Application.persistentDataPath, logFilePath);;

            try
            {
                // FileShare.Read ���w�肵�āA���̃v���Z�X���ǂݎ��\�ɂ���
                _fileStream = new FileStream(_logFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                _binaryReader = new BinaryReader(_fileStream);

                // LogHeader ��ǂݍ���
                // 4�o�C�g�̒����v���t�B�b�N�X��ǂݍ���
                if (_binaryReader.BaseStream.Length < 4)
                {
                    UnityEngine.Debug.LogError("���O�t�@�C�����Z�����܂��B�w�b�_�̒����v���t�B�b�N�X���ǂݎ��܂���: " + _logFilePath);
                    Dispose();
                    return false;
                }
                int headerLength = _binaryReader.ReadInt32();

                if (_binaryReader.BaseStream.Length < _binaryReader.BaseStream.Position + headerLength)
                {
                    UnityEngine.Debug.LogError("���O�t�@�C���̃w�b�_�������j�����Ă��邩�A�s���S�ł�: " + _logFilePath);
                    Dispose();
                    return false;
                }
                byte[] headerBytes = _binaryReader.ReadBytes(headerLength);

                LogHeader = _logHeaderParser.ParseFrom(headerBytes);
                if (LogHeader == null)
                {
                    UnityEngine.Debug.LogError("LogHeader �̃f�V���A���C�Y�Ɏ��s���܂���: " + _logFilePath);
                    Dispose();
                    return false;
                }

                // �ŏ��̃t���[���f�[�^�̈ʒu���L������
                _nextFrameOffset = _fileStream.Position;
                _isInitialized = true;
                UnityEngine.Debug.Log("LandmarkProtoBinaryLogReader ������������܂����B");
                return true;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"LandmarkProtoBinaryLogReader �̏������Ɏ��s���܂���: {ex.Message}");
                Dispose();
                return false;
            }
        }

        public void StartPlayback()
        {
            if (!_isInitialized)
            {
                UnityEngine.Debug.LogWarning("���[�_�[������������Ă��܂���BInitialize() ���ɌĂяo���Ă��������B");
                return;
            }
            _isPlaying = true;
            UnityEngine.Debug.Log("�Đ����J�n���܂����B");
        }

        public void StopPlayback()
        {
            _isPlaying = false;
            UnityEngine.Debug.Log("�Đ����~���܂����B");
        }

        public void ResetPlayback()
        {
            if (!_isInitialized) return;
            StopPlayback(); // �Đ����~
            _fileStream.Seek(_nextFrameOffset, SeekOrigin.Begin); // �w�b�_�̒���ɖ߂�
            _firstTimestampMillis = null; // �ŏ��̃^�C���X�^���v�����Z�b�g
            _currentFrameDataBuffer = null; // �o�b�t�@���N���A
            UnityEngine.Debug.Log("�Đ��������߂��܂����B");
        }

        /// <summary>
        /// ���t���[���Ăяo����A���O�f�[�^��ǂݍ��݁A�C�x���g�𔭉΂���B
        /// </summary>
        /// <param name="deltaTime">�O�t���[������̌o�ߎ��� (�b)�B</param>
        public async UniTask Update(float deltaTime) // async void �̓G���[�n���h�����O�ɒ��ӂ��K�v
        {
            if (!_isInitialized || !_isPlaying) return;

            // ���A���^�C���ƍĐ����x�Ɋ�Â��ă^�[�Q�b�g�Đ����Ԃ�i�߂�
            // deltaTime �͕b�Ȃ̂ŁA�~���b�ɕϊ�
            // PlaybackSpeed �͂����ŏ�Z���AGetRelativeTimestamp�ł͏����Ȏ��ԍ�������
            double currentUnityTimeMs = (UnityEngine.Time.timeAsDouble * 1000.0 * PlaybackSpeed);

            // �^�[�Q�b�g���Ԃɒǂ����܂ŁA�܂��̓t�@�C���̏I�[�ɒB����܂Ńt���[����ǂݍ���
            while (_isInitialized && _isPlaying)
            {
                LogFrameData frameToProcess = _currentFrameDataBuffer;
                if (frameToProcess == null)
                {
                    // ���̃t���[���f�[�^��񓯊��œǂݍ���
                    bool _;
                    (_, frameToProcess) = await ReadNextFrameUniTaskAsync().SuppressCancellationThrow(); // �L�����Z����O��}��

                    if (frameToProcess == null)
                    {
                        UnityEngine.Debug.Log("���O�t�@�C���̏I�[�ɒB���܂����B");
                        _isPlaying = false; // �t�@�C���I�[�ōĐ����~
                        _currentFrameDataBuffer = null;
                        break;
                    }

                    // �ŏ��̃t���[���^�C���X�^���v���L�^
                    if (!_firstTimestampMillis.HasValue)
                    {
                        _firstTimestampMillis = frameToProcess.TimestampMs;
                    }
                }

                // �ŏ��̃t���[������̑��΃^�C���X�^���v���v�Z
                // ���� `relativeTimestampMs` �́A���O�̋L�^������̌o�ߎ��� (�~���b)
                double relativeTimestampMs = frameToProcess.TimestampMs - _firstTimestampMillis.Value;

                // Unity�̎��Ԃƃ��O�̑��Ύ��Ԃ��r
                if (relativeTimestampMs <= currentUnityTimeMs)
                {
                    // ���݂�Unity���ԂɊԂɍ����t���[���Ȃ̂ŏ�������
                    // �C�x���g����: HolisticLandmarks �� float �^�̑��΃^�C���X�^���v (�~���b)
                    OnLandmarksReceived?.Invoke(frameToProcess.HolisticLandmarks, (float)relativeTimestampMs);
                    _currentFrameDataBuffer = null; // ���̃t���[���͏������ꂽ�̂Ńo�b�t�@���N���A
                    // UnityEngine.Debug.Log($"�t���[�� {frameToProcess.FrameNumber} ���������܂��� ({frameToProcess.TimestampMs}ms, ����: {(float)relativeTimestampMs}ms)");
                }
                else
                {
                    // ���̃t���[����Unity�̌��݂̎��Ԃ��i��ł���̂ŁA����̂��߂Ƀo�b�t�@���đ҂�
                    _currentFrameDataBuffer = frameToProcess;
                    break; // ���� Update �T�C�N���ł��̃t���[����҂�
                }
            }
        }

        /// <summary>
        /// ���̃��O�t���[���f�[�^��񓯊��œǂݍ��� (UniTask ��)�B�t�@�C���̏I�[�ɒB������ null ��Ԃ��B
        /// </summary>
        private async UniTask<LogFrameData> ReadNextFrameUniTaskAsync()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("���[�_�[������������Ă��܂���BInitialize ���ɌĂяo���Ă��������B");
            }

            // UniTask.Yield() �����ނ��ƂŁA���̃t���[���ǂݍ��݂�ҋ@����Unity���t���[�Y���Ȃ��悤�ɂ���
            // ���ɑ傫�ȃt�@�C����ǂݍ��ޏꍇ�ȂǂɗL���B
            await UniTask.Yield(PlayerLoopTiming.Update); // �܂��� FixedUpdate, PostLateUpdate �ȂǓK�؂�

            // ���̃t���[���f�[�^�̈ʒu����ǂݍ��݂��ĊJ����
            _fileStream.Seek(_nextFrameOffset, SeekOrigin.Begin);

            if (_fileStream.Position + 4 > _fileStream.Length)
            {
                // �t�@�C���̏I�[�ɒB�������A���̒����v���t�B�b�N�X���ǂݎ��Ȃ�
                return null;
            }

            byte[] lengthBytes = _binaryReader.ReadBytes(4);
            if (lengthBytes.Length < 4) return null; // �ǂݍ��߂Ȃ������ꍇ
            int frameLength = BitConverter.ToInt32(lengthBytes, 0);

            if (_fileStream.Position + frameLength > _fileStream.Length)
            {
                UnityEngine.Debug.LogError($"�x��: �t���[���f�[�^���s���S�ł��B���҂���钷��: {frameLength}, �c��o�C�g: {_fileStream.Length - _fileStream.Position}");
                return null;
            }

            byte[] frameBytes = _binaryReader.ReadBytes(frameLength);

            LogFrameData frameData = _logFrameDataParser.ParseFrom(frameBytes);

            // ���̓ǂݍ��݂̂��߂ɃX�g���[���ʒu���X�V����
            _nextFrameOffset = _fileStream.Position;

            return frameData;
        }

        /// <summary>
        /// ���\�[�X���������B
        /// </summary>
        public void Dispose()
        {
            _binaryReader?.Close();
            _binaryReader?.Dispose();
            _fileStream?.Close();
            _fileStream?.Dispose();
            _binaryReader = null;
            _fileStream = null;
            _isInitialized = false;
            _isPlaying = false;
            UnityEngine.Debug.Log("LandmarkProtoBinaryLogReader �� Dispose ����܂����B");
        }
    }
}