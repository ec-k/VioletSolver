using UnityEngine;
using RootMotion.FinalIK;

namespace VioletSolver
{
    public static class ArmIKSetup
    {
        public static void Initialize(
            GameObject ikRigRoot,
            Animator animator,
            out Transform leftShoulderTarget,
            out Transform leftElbowTarget,
            out Transform leftHandTarget,
            out Transform rightShoulderTarget,
            out Transform rightElbowTarget,
            out Transform rightHandTarget,
            out ArmIK leftArmIk,
            out ArmIK rightArmIk)
        {
            // Setup IK targets.
            var targetRoot = new GameObject("ArmIK Targets").transform;
            targetRoot.parent = ikRigRoot.transform;
            targetRoot.localPosition = Vector3.zero;
            targetRoot.localRotation = Quaternion.identity;

            leftShoulderTarget  = SetupIkTargetInternal(targetRoot, "LeftShoulderTarget");
            leftElbowTarget     = SetupIkTargetInternal(targetRoot, "LeftElbowTarget");
            leftHandTarget      = SetupIkTargetInternal(targetRoot, "LeftHandTarget");
            rightShoulderTarget = SetupIkTargetInternal(targetRoot, "RightShoulderTarget");
            rightElbowTarget    = SetupIkTargetInternal(targetRoot, "RightElbowTarget");
            rightHandTarget     = SetupIkTargetInternal(targetRoot, "RightHandTarget");

            // Setup ArmIK components.
            leftArmIk = SetupArmIkComponent(ikRigRoot, animator, true, leftHandTarget, leftElbowTarget);
            rightArmIk = SetupArmIkComponent(ikRigRoot, animator, false, rightHandTarget, rightElbowTarget);
        }

        static Transform SetupIkTargetInternal(Transform root, string name)
        {
            var target = new GameObject(name).transform;
            target.parent = root;
            return target;
        }

        static ArmIK SetupArmIkComponent(
            GameObject ikRigRoot,
            Animator animator,
            bool isLeft, 
            Transform handTarget,
            Transform elbowTarget)
        {
            var armIk = ikRigRoot.AddComponent<ArmIK>();

            armIk.solver.IKPositionWeight = 1f;
            armIk.solver.IKRotationWeight = 1f;
            armIk.solver.arm.bendGoalWeight = 1f;
            armIk.solver.chest.transform = animator.GetBoneTransform(HumanBodyBones.Chest);

            if (isLeft)
            {
                armIk.solver.shoulder.transform = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
                armIk.solver.upperArm.transform = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
                armIk.solver.forearm.transform = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                armIk.solver.hand.transform = animator.GetBoneTransform(HumanBodyBones.LeftHand);

                armIk.solver.arm.target = handTarget;
                armIk.solver.arm.bendGoal = elbowTarget;
                armIk.solver.isLeft = true;
            }
            else
            {
                armIk.solver.shoulder.transform = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
                armIk.solver.upperArm.transform = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
                armIk.solver.forearm.transform = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
                armIk.solver.hand.transform = animator.GetBoneTransform(HumanBodyBones.RightHand);

                armIk.solver.arm.target = handTarget;
                armIk.solver.arm.bendGoal = elbowTarget;
                armIk.solver.isLeft = false;
            }
            return armIk;
        }
    }
}