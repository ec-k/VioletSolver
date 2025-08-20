using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using VRM;

using BlendshapeIndex = VioletSolver.Recorder.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver.Recorder
{
    public class AvatarMotionRecorder
    {
        readonly Animator _avatarAnimator;
        readonly VRMBlendShapeProxy _blendShapeProxy;
        readonly string _avatarName;
        readonly string _avatarVersion;

        const string LogSchemaVersion = "0.1";

        uint _frameNumber = 1;
        Dictionary<BlendShapeKey, float> _blendshapes;
        FileStream _fileStream;
        BinaryWriter _writer;
        string _logFilePath;
        bool _isInitialized = false;
        bool _isRecording = false;

        Timer _timer = new();

        public AvatarMotionRecorder(Animator avatarAnimator, VRMBlendShapeProxy blendShapeProxy, string avatarName, string avatarVersion)
        {
            _avatarAnimator = avatarAnimator ?? throw new ArgumentNullException(nameof(avatarAnimator));
            _blendShapeProxy = blendShapeProxy ?? throw new ArgumentNullException(nameof(blendShapeProxy));
            _avatarName = avatarName ?? throw new ArgumentNullException(nameof(avatarName));
            _avatarVersion = avatarVersion ?? throw new ArgumentNullException(nameof(avatarVersion));
        }

        public bool Initialzie(string logFilePath)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("AvatarMotionRecorder is already initialized.");
                return true;
            }

            _blendshapes = _blendShapeProxy.GetValues().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            _logFilePath = Path.Combine(Application.persistentDataPath, logFilePath);
            try
            {
                _fileStream = new FileStream(_logFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                _writer = new BinaryWriter(_fileStream, System.Text.Encoding.UTF8, true);
                _isInitialized = true;
                WriteHeader();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create a log file: {e.Message}");
                return false;
            }
        }

        public void StartRecording()
        {
            if (!_isInitialized)
            {
                Debug.LogError("AvatarMotionRecorder is not initialized. Call Initialize() first.");
                return;
            }
            _timer.Start();
            _isRecording = true;
        }

        public void PauseRecording()
        {
            if (!_isInitialized)
            {
                Debug.LogError("AvatarMotionRecorder is not initialized. Call Initialize() first.");
                return;
            }
            _timer.Pause();
            _isRecording = false;
        }

        void WriteHeader()
        {
            if (_fileStream.Length > 0)
            {
                Debug.LogWarning("Log file already contains data. Header will not be written again.");
                return;
            }

            var header = new LogHeader
            {
                LogSchemaVersion = LogSchemaVersion,
                AvatarName = _avatarName,
                AvatarVersion = _avatarVersion,
            };
            var headerBytes = header.ToByteArray();
            _writer.Write(headerBytes);
        }

        public void Update()
        {
            _timer.Update();
            if (!_isInitialized || !_isRecording) return;

            var boneTransforms = Enum.GetValues(typeof(HumanBodyBones))
                .Cast<HumanBodyBones>()
                .Where(boneType => boneType != HumanBodyBones.LastBone)
                .Select(boneType =>
                {
                    var transform = _avatarAnimator.GetBoneTransform(boneType);
                    return new {
                        BoneType = boneType,
                        Transform = transform,
                    };
                })
                .Where(bone => bone.Transform != null)
                .Select(bone =>
                {
                    var position = new Position
                    {
                        X = bone.Transform.localPosition.x,
                        Y = bone.Transform.localPosition.y,
                        Z = bone.Transform.localPosition.z
                    };

                    var rotation = new Rotation
                    {
                        X = bone.Transform.localRotation.x,
                        Y = bone.Transform.localRotation.y,
                        Z = bone.Transform.localRotation.z,
                        W = bone.Transform.localRotation.w
                    };

                    var boneTransform = new BoneTransform
                    {
                        BoneTypeValue = (int)bone.BoneType,
                        BoneName = bone.BoneType.ToString(),
                        LocalPosition = position,
                        LocalRotation = rotation
                    };

                    return boneTransform;
                })
                .ToList();

            var dictionary = _blendshapes
                .Where(kvp => Enum.TryParse<BlendshapeIndex>(kvp.Key.ToString(), out _))
                .Select(kvp =>
                {
                    var index = Enum.Parse<BlendshapeIndex>(kvp.Key.ToString());
                    return new
                    {
                        Key = index,
                        Value = kvp.Value
                    };
                })
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var weights = new float[(int)BlendshapeIndex.Length];
            foreach (var kvp in dictionary)
            {
                var index = (int)kvp.Key;
                weights[index] = kvp.Value;
            }
            var bledshapeWeights = new Blendshapes
            {
                Weights = { weights }
            };

            var avatarPose = new AvatarPose
            {
                BoneTransforms = { boneTransforms },
                Blendshapes = bledshapeWeights
            };

            var deviceUsage = new DeviceUsage
            {
                IsMouseUsing = false,
                IsKeyboardUsing = false,
            };

            var frameData = new LogFrameData
            {
                TimestampMs = _timer.CurrentTimeMs,
                FrameNumber = _frameNumber,
                AvatarPose = avatarPose,
                DeviceUsage = deviceUsage,
            };
            _frameNumber++;

            _writer.Write(frameData.ToByteArray());
        }

        public void StopRecording()
        {
            if (!_isInitialized)
            {
                Debug.LogError("AvatarMotionRecorder is not initialized. Call Initialize() first.");
                return;
            }
            _timer.Pause();
            _timer.Reset();
            _writer.Flush();
            Dispose();
            Debug.Log($"AvatarMotionRecorder stopped recording. Total frames: {_frameNumber - 1}");
        }

        public void Dispose()
        {
            _writer?.Close();
            _writer?.Dispose();
            _writer = null;
            _fileStream?.Close();
            _fileStream?.Dispose();
            _fileStream = null;
            _isInitialized = false;
            _isRecording = false;
            Debug.Log($"AvatarMotionRecorder disposed");
        }
    }
}
