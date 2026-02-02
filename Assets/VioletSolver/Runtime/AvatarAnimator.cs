using RootMotion.FinalIK;
using System.Collections.Generic;
using UnityEngine;
using VioletSolver.Pose;
using VioletSolver.Solver;
using VRM;
using mpBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

// This is avatar animating component which does
//  1. gets landmarks and filters landmarks (in _landmarkHandler)
//  2. solve landmarks to avatar pose
//  3. filters pose and apply pose to avatar (in _avatarPoseHandler)
namespace VioletSolver
{
    public class AvatarAnimator
    {
        public Animator Animator { get; set; }
        readonly GameObject _ikRigRoot;

        readonly LandmarkHandler _landmarkHandler;
        readonly PoseHandler _avatarPoseHandler;
        readonly AvatarBonePositions _restBonePositions;
        readonly AvatarBoneRotations _restBoneRotations;

        readonly IBlendshapeSolver _blendshapeSolver;
        readonly IFaceApplier _faceApplier;

        readonly VRIK _vrik;

        /// <summary>
        /// Initializes an AvatarAnimator and sets up the VRIK component.
        /// </summary>
        /// <param name="ikRigRoot">The root GameObject for the IK rig.</param>
        /// <param name="animator">The avatar's Animator component.</param>
        /// <param name="landmarkHandler">The handler that receives and filters landmarks.</param>
        /// <param name="blendshapeSolver">The solver that resolves blendshapes.</param>
        /// <param name="faceApplier">The applier that applies facial expressions to the avatar.</param>
        public AvatarAnimator(
            GameObject ikRigRoot,
            Animator animator,
            LandmarkHandler landmarkHandler,
            IBlendshapeSolver blendshapeSolver,
            IFaceApplier faceApplier)
        {
            _landmarkHandler = landmarkHandler;
            _avatarPoseHandler = new();

            _blendshapeSolver = blendshapeSolver;
            _faceApplier = faceApplier;

            _ikRigRoot = ikRigRoot;
            Animator = animator;

            _restBonePositions = AvatarBonePositionsInitializer.CreateFromAnimator(animator);
            _restBoneRotations = AvatarBoneRotationsInitializer.CreateFromAnimator(animator);

            VRIKSetup.Initialize(
                _ikRigRoot,
                Animator,
                out _vrik);
        }

        /// <summary>
        /// Calculates pose and blendshape data from landmarks and returns the animation result.
        /// </summary>
        /// <param name="isIkEnabled">Whether to use IK.</param>
        /// <returns>The calculated animation result data.</returns>
        public AnimationResultData CalculateAnimationData(bool isIkEnabled)
        {
            UpdatePose(isIkEnabled);

            Dictionary<BlendShapePreset, float> vrmBs = null;
            IReadOnlyDictionary<mpBlendshapes, float> mpBs = null;

            var bsResult = _blendshapeSolver.Solve(_landmarkHandler.MpBlendshapes);
            if (bsResult is not null)
            {
                if (bsResult.VrmBlendshapes is not null)
                {
                    _avatarPoseHandler.Update(bsResult.VrmBlendshapes);
                    vrmBs = _avatarPoseHandler.BlendshapeWeights;
                }

                if (bsResult.PerfectSyncBlendshapes is not null)
                {
                    _avatarPoseHandler.Update(bsResult.PerfectSyncBlendshapes);
                    mpBs = _avatarPoseHandler.PerfectSyncWeights;
                }

                _avatarPoseHandler.Update(HumanBodyBones.LeftEye, bsResult.LeftEye);
                _avatarPoseHandler.Update(HumanBodyBones.RightEye, bsResult.RightEye);
            }

            return new AnimationResultData
            {
                PoseData = _avatarPoseHandler.PoseData,
                VrmBlendshapes = vrmBs,
                PerfectSyncBlendshapes = mpBs
            };
        }

        /// <summary>
        /// Applies animation data to the avatar.
        /// </summary>
        /// <param name="data">The animation result data to apply.</param>
        /// <param name="isIkEnabled">Whether to use IK.</param>
        /// <param name="enableLeg">Whether to enable leg animation.</param>
        /// <param name="offset">Position and rotation offset for the avatar.</param>
        public void ApplyAnimationData(AnimationResultData data, bool isIkEnabled, bool enableLeg, Transform? offset = null)
        {
            _vrik.enabled = isIkEnabled;

            AnimateAvatar(Animator, data.PoseData, isIkEnabled, enableLeg, offset);

            if (data.VrmBlendshapes is not null)
                _faceApplier.Apply(data.VrmBlendshapes);
            else if (data.PerfectSyncBlendshapes is not null)
                _faceApplier.Apply(data.PerfectSyncBlendshapes);
        }

        /// <summary>
        /// </summary>
        /// <returns>Whether updating landmarks is processed properly or not.</returns>
        void UpdatePose(bool isIkEnabled)
        {
            var isKinectPose = _landmarkHandler.IsKinectPose;

            var landmarks = _landmarkHandler.Landmarks;
            var pose = HolisticSolver.Solve(landmarks, _restBonePositions, _restBoneRotations, isIkEnabled, isKinectPose);
            pose.time = isKinectPose ? landmarks.KinectPose.Time : landmarks.MediaPipePose.Time;
            _avatarPoseHandler.Update(pose);
        }

        void AnimateAvatar(Animator animator, AvatarPoseData pose, bool isIkEnabled, bool enableLeg, Transform? offset = null)
        {
            if (!enableLeg)
                foreach (var bone in BodyPartsBones.Legs)
                    pose[bone] = Quaternion.identity;

            if (isIkEnabled)
            {
                // IK mode: VRIK handles spine, arms, and legs.
                ApplyIkTarget(pose, enableLeg, offset);
            }
            else
            {
                // Non-IK mode: Apply rotations directly.
                if (offset is not null)
                    animator.GetBoneTransform(HumanBodyBones.Hips).position = offset.rotation * pose.HipsPosition + offset.position;
                else
                    animator.GetBoneTransform(HumanBodyBones.Hips).position = pose.HipsPosition;

                foreach (var bone in BodyPartsBones.Spines)
                    ApplyGlobal(animator, pose, bone, offset);

                foreach (var bone in BodyPartsBones.Legs)
                    ApplyGlobal(animator, pose, bone, offset);

                foreach (var bone in BodyPartsBones.Arms)
                    ApplyGlobal(animator, pose, bone, offset);
            }

            // Fingers and eyes are not affected by IK.
            foreach (var bone in BodyPartsBones.Fingers)
                ApplyLocal(animator, pose, bone);

            foreach (var bone in BodyPartsBones.Eyes)
                ApplyLocal(animator, pose, bone);
        }

        void ApplyIkTarget(AvatarPoseData pose, bool enableLeg, Transform? offset = null)
        {
            if (offset is not null)
            {
                _ikRigRoot.transform.position = offset.position;
                _ikRigRoot.transform.rotation = offset.rotation;
            }
            else
            {
                _ikRigRoot.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }

            // Set all IK targets first.
            _vrik.solver.spine.headTarget.localPosition = pose.HeadPosition;
            _vrik.solver.spine.headTarget.localRotation = pose.Head;
            _vrik.solver.spine.pelvisTarget.localPosition = pose.HipsPosition;
            _vrik.solver.spine.pelvisTarget.localRotation = pose.Hips;

            _vrik.solver.leftArm.target.localPosition = pose.LeftHandPosition;
            _vrik.solver.leftArm.target.localRotation = pose.LeftHand;
            _vrik.solver.leftArm.bendGoal.localPosition = pose.LeftElbowPosition;

            _vrik.solver.rightArm.target.localPosition = pose.RightHandPosition;
            _vrik.solver.rightArm.target.localRotation = pose.RightHand;
            _vrik.solver.rightArm.bendGoal.localPosition = pose.RightElbowPosition;

            var legWeight = enableLeg ? 1f : 0f;
            _vrik.solver.leftLeg.positionWeight = legWeight;
            _vrik.solver.leftLeg.rotationWeight = legWeight;
            _vrik.solver.leftLeg.bendGoalWeight = legWeight;
            _vrik.solver.rightLeg.positionWeight = legWeight;
            _vrik.solver.rightLeg.rotationWeight = legWeight;
            _vrik.solver.rightLeg.bendGoalWeight = legWeight;

            if (enableLeg)
            {
                _vrik.solver.leftLeg.target.localPosition = pose.LeftFootPosition;
                _vrik.solver.leftLeg.target.localRotation = pose.LeftFoot;
                _vrik.solver.leftLeg.bendGoal.localPosition = pose.LeftKneePosition;

                _vrik.solver.rightLeg.target.localPosition = pose.RightFootPosition;
                _vrik.solver.rightLeg.target.localRotation = pose.RightFoot;
                _vrik.solver.rightLeg.bendGoal.localPosition = pose.RightKneePosition;
            }

            // Calculate the avatar root position.
            // Use head as the horizontal reference, and place root below all joints.
            var headWorldPos = _vrik.solver.spine.headTarget.position;
            var minY = Mathf.Min(
                _vrik.solver.spine.headTarget.position.y,
                _vrik.solver.spine.pelvisTarget.position.y,
                _vrik.solver.leftArm.target.position.y,
                _vrik.solver.rightArm.target.position.y
            );
            if (enableLeg)
            {
                minY = Mathf.Min(
                    minY,
                    _vrik.solver.leftLeg.target.position.y,
                    _vrik.solver.rightLeg.target.position.y
                );
            }

            // Place root below all joints with a small margin.
            Animator.transform.position = new Vector3(headWorldPos.x, minY - 0.1f, headWorldPos.z);
            Animator.transform.rotation = offset?.rotation ?? Quaternion.identity;
        }

        void ApplyLocal(Animator animator, AvatarPoseData pose, HumanBodyBones boneName)
            => animator.GetBoneTransform(boneName).localRotation = pose[boneName];

        void ApplyGlobal(Animator animator, AvatarPoseData pose, HumanBodyBones boneName, Transform? offset = null)
        {
            if (offset is not null)
                animator.GetBoneTransform(boneName).rotation = offset.rotation * pose[boneName];
            else
                animator.GetBoneTransform(boneName).rotation = pose[boneName];
        }
    }
}
