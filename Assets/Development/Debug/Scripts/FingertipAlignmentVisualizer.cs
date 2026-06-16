using UnityEngine;

namespace VioletSolver.Debug
{
    /// <summary>
    /// Visualizes the target DIP position calculated by FingertipAlignmentSolver.
    /// Attach to any GameObject in the scene and assign references in the Inspector.
    /// </summary>
    public class FingertipAlignmentVisualizer : MonoBehaviour
    {
        [SerializeField] Animator _animator;
        [SerializeField] Transform _leftWristIkTarget;
        [SerializeField] Transform _rightWristIkTarget;

        [Header("Gizmo Settings")]
        [SerializeField] float _sphereRadius = 0.02f;
        [SerializeField] bool _enableTargetIndexDip = true;
        [SerializeField] bool _enableDistalTf = true;

        void OnDrawGizmos()
        {
            DrawDipGizmo(_leftWristIkTarget, HumanBodyBones.LeftHand, HumanBodyBones.LeftIndexDistal, Color.cyan);
            DrawDipGizmo(_rightWristIkTarget, HumanBodyBones.RightHand, HumanBodyBones.RightIndexDistal, Color.yellow);
        }

        void DrawDipGizmo(Transform wristTarget, HumanBodyBones wristBone, HumanBodyBones distalBone, Color color)
        {
            if (wristTarget == null || _animator == null) return;

            var wristTf = _animator.GetBoneTransform(wristBone);
            var distalTf = _animator.GetBoneTransform(distalBone);
            if (wristTf == null || distalTf == null) return;

            // targetDIP = ikWristPosition + avatarWristToDistal (in world space)
            var targetDip = wristTarget.position + (distalTf.position - wristTf.position);

            // Target DIP: where FingertipAlignmentSolver intends the DIP to be
            if (_enableTargetIndexDip)
            {
                Gizmos.color = color;
                Gizmos.DrawSphere(targetDip, _sphereRadius);
                Gizmos.DrawLine(wristTarget.position, targetDip);
            }

            // Actual DIP: avatar's current index distal bone position
            if (_enableDistalTf)
            {
                var actualColor = color * 0.5f;
                actualColor.a = 1f;
                Gizmos.color = actualColor;
                Gizmos.DrawSphere(distalTf.position, _sphereRadius);
                Gizmos.DrawLine(wristTf.position, distalTf.position);
            }
        }
    }
}
