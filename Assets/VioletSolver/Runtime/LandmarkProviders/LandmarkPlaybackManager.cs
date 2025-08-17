using System;
using UnityEngine;

// �������ꂽProtobuf C#�N���X��using (�v���W�F�N�g�̍\���ɂ���ĈقȂ�ꍇ������)
using HumanLandmarks;
using HumanLandmarks.Log;

namespace VioletSolver.LandmarkProviders
{
    /// <summary>
    /// �A�o�^�[�̎p���Đ����Ǘ�����Unity�R���|�[�l���g�B
    /// ILandmarkProvider �C���^�[�t�F�[�X���������AILandmarkLogReader ���g�p���ă��O�f�[�^��ǂݍ��ށB
    /// </summary>
    public class LandmarkPlaybackManager : MonoBehaviour, ILandmarkProvider
    {
        // ILandmarkProvider �C���^�[�t�F�[�X�̃C�x���g����
        public event Action<HumanLandmarks.HolisticLandmarks, float> OnLandmarksReceived;

        [Tooltip("�Đ����郍�O�t�@�C���̃p�X�B�A�Z�b�g�p�X�܂��͐�΃p�X�B")]
        public string logFilePath;

        [Tooltip("���O�̍Đ����x (1.0�Ń��A���^�C��)�B")]
        [Range(0.1f, 10.0f)]
        public float playbackSpeed = 1.0f;

        // ILandmarkLogReader �̃C���X�^���X��ێ�
        private ILandmarkLogReader _logFileReader;

        // ILandmarkProvider �C���^�[�t�F�[�X�̃v���p�e�B����
        public bool IsPlayingLog => _logFileReader?.IsPlaying ?? false;

        public float CurrentPlaybackSpeed
        {
            get => _logFileReader?.PlaybackSpeed ?? 0f;
            set
            {
                playbackSpeed = value; // �C���X�y�N�^�̒l���X�V
                if (_logFileReader != null)
                {
                    _logFileReader.PlaybackSpeed = value;
                }
            }
        }

        // ILandmarkProvider �C���^�[�t�F�[�X�ɒǉ����ꂽ LogHeader �v���p�e�B
        // _logFileReader ������������Ă���΂��� LogHeader ��Ԃ�
        public LogHeader LogHeader => _logFileReader?.LogHeader;


        // �O�����琧�䂷�邽�߂̃p�u���b�N���\�b�h (ILandmarkProvider �C���^�[�t�F�[�X�̈ꕔ)
        public void Play() => _logFileReader?.StartPlayback();
        public void Pause() => _logFileReader?.StopPlayback();
        public void Rewind() => _logFileReader?.ResetPlayback();

        // Unity�̃��C�t�T�C�N�����\�b�h
        void Awake()
        {
            // �����ŋ�̓I�Ȏ����̃��O���[�_�[���C���X�^���X������
            // �R���X�g���N�^�Ńp�X��n���K�v�͂Ȃ��Ȃ����̂ŁAInitialize���\�b�h�Őݒ�
            _logFileReader = new LandmarkProtoBinaryLogReader(); // �p�X��n���Ȃ��R���X�g���N�^���g�p

            // _logFileReader����̃C�x���g���t�b�N
            _logFileReader.OnLandmarksReceived += OnLogLandmarksReceived;
        }

        void Start()
        {
            OnLandmarksReceived += LoggingForDebug;
            // �C���X�y�N�^�Őݒ肳�ꂽ�p�X��POCO��������
            // Initialize���\�b�h�� bool ��Ԃ��̂ŁA���̌��ʂ��m�F����
            bool isInitialized = _logFileReader.Initialize(logFilePath);

            if (isInitialized)
            {
                _logFileReader.PlaybackSpeed = playbackSpeed; // Initialize���PlaybackSpeed��ݒ�
                // �V�[���J�n���Ɏ����ōĐ����J�n�������ꍇ
                _logFileReader.StartPlayback();
            }
            else
            {
                Debug.LogError($"���O�t�@�C�����[�_�[�̏������Ɏ��s���܂����B�p�X: {logFilePath}");
                // ���������s���͂��̃R���|�[�l���g�𖳌������āAUpdate���Ă΂�Ȃ��悤�ɂ���
                enabled = false;
            }
        }

        void LoggingForDebug(HumanLandmarks.HolisticLandmarks result, float timeMs)
        {
            UnityEngine.Debug.Log($"Received time: {timeMs}");
        }

        void Update()
        {
            // �R���|�[�l���g������������Ă���ꍇ�͉������Ȃ�
            if (!enabled) return;

            // _logFileReader��Update���\�b�h�𖈃t���[���Ăяo��
            _logFileReader?.Update(Time.deltaTime);

            // ������UI����̑���Ȃǂ����m���A_logFileReader.StartPlayback(), StopPlayback(), ResetPlayback() ���Ăяo��
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (IsPlayingLog)
                {
                    _logFileReader?.StopPlayback();
                }
                else
                {
                    _logFileReader?.StartPlayback();
                }
            }
            if (Input.GetKeyDown(KeyCode.R)) // R�L�[�Ń��Z�b�g
            {
                _logFileReader?.ResetPlayback();
            }
        }

        // _logFileReader����󂯎�����C�x���g�����̂܂܊O���ɔ���
        private void OnLogLandmarksReceived(HumanLandmarks.HolisticLandmarks landmarks, float relativeTime)
        {
            OnLandmarksReceived?.Invoke(landmarks, relativeTime);
        }

        // ���\�[�X�����
        void OnDestroy()
        {
            if (_logFileReader is not null)
            {
                // �C�x���g�̍w�ǂ�����
                _logFileReader.OnLandmarksReceived -= OnLogLandmarksReceived;
                // ���\�[�X�����
                _logFileReader.Dispose();
                _logFileReader = null;
            }
        }
    }
}