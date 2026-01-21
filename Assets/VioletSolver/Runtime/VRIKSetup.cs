using RootMotion.FinalIK;
using UnityEngine;

namespace VioletSolver
{
    public static class VRIKSetup
    {
        public static void Initialize(
            GameObject ikRigRoot,
            Animator animator,
            out VRIK vrik)
        {
            // Setup IK targets.
            var targetRoot = new GameObject("VRIK Targets").transform;
            targetRoot.parent = ikRigRoot.transform;
            targetRoot.localPosition = Vector3.zero;
            targetRoot.localRotation = Quaternion.identity;

            var headTarget = SetupIkTargetInternal(targetRoot, "HeadTarget");
            var pelvisTarget = SetupIkTargetInternal(targetRoot, "PelvisTarget");
            var leftHandTarget = SetupIkTargetInternal(targetRoot, "LeftHandTarget");
            var leftElbowTarget = SetupIkTargetInternal(targetRoot, "LeftElbowTarget");
            var rightHandTarget = SetupIkTargetInternal(targetRoot, "RightHandTarget");
            var rightElbowTarget = SetupIkTargetInternal(targetRoot, "RightElbowTarget");
            var leftFootTarget = SetupIkTargetInternal(targetRoot, "LeftFootTarget");
            var leftKneeTarget = SetupIkTargetInternal(targetRoot, "LeftKneeTarget");
            var rightFootTarget = SetupIkTargetInternal(targetRoot, "RightFootTarget");
            var rightKneeTarget = SetupIkTargetInternal(targetRoot, "RightKneeTarget");

            // Setup VRIK component.
            vrik = animator.gameObject.AddComponent<VRIK>();
            vrik.AutoDetectReferences();
            vrik.solver.locomotion.weight = 0f;

            // Spine targets.
            vrik.solver.spine.headTarget = headTarget;
            vrik.solver.spine.pelvisTarget = pelvisTarget;
            vrik.solver.spine.pelvisPositionWeight = 1f;
            vrik.solver.spine.pelvisRotationWeight = 1f;

            // Left arm targets.
            vrik.solver.leftArm.target = leftHandTarget;
            vrik.solver.leftArm.bendGoal = leftElbowTarget;
            vrik.solver.leftArm.bendGoalWeight = 1f;

            // Right arm targets.
            vrik.solver.rightArm.target = rightHandTarget;
            vrik.solver.rightArm.bendGoal = rightElbowTarget;
            vrik.solver.rightArm.bendGoalWeight = 1f;

            // Left leg targets (weight controlled at runtime).
            vrik.solver.leftLeg.target = leftFootTarget;
            vrik.solver.leftLeg.positionWeight = 0f;
            vrik.solver.leftLeg.rotationWeight = 0f;
            vrik.solver.leftLeg.bendGoal = leftKneeTarget;
            vrik.solver.leftLeg.bendGoalWeight = 0f;

            // Right leg targets (weight controlled at runtime).
            vrik.solver.rightLeg.target = rightFootTarget;
            vrik.solver.rightLeg.positionWeight = 0f;
            vrik.solver.rightLeg.rotationWeight = 0f;
            vrik.solver.rightLeg.bendGoal = rightKneeTarget;
            vrik.solver.rightLeg.bendGoalWeight = 0f;

            vrik.solver.plantFeet = false;
        }

        static Transform SetupIkTargetInternal(Transform root, string name)
        {
            var target = new GameObject(name).transform;
            target.parent = root;
            return target;
        }
    }
}
