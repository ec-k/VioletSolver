using UnityEngine;

namespace VioletSolver
{
    /// <summary>
    /// This class prevents no-rendering problem that caused by positional difference of rig and its related renderers' bounds.
    /// </summary>
    public class AssetsPositionAdjuster
    {
        public Animator Animator;
        public SkinnedMeshRenderer Face;
        public SkinnedMeshRenderer Body;

        public void Adjust()
        {
            if (Face != null) AdjustOneBounds(Face, Animator, HumanBodyBones.Head);
            if (Body != null) AdjustOneBounds(Body, Animator, HumanBodyBones.Hips);
        }

        void AdjustOneBounds(SkinnedMeshRenderer target, Animator avatarAnimator, HumanBodyBones rootBoneType)
        {
            var bone = avatarAnimator.GetBoneTransform(rootBoneType);
            if (bone != null)
            {
                target.rootBone = bone;

                var recenteredLocalBounds = target.localBounds;
                recenteredLocalBounds.center = Vector3.zero;
                target.localBounds = recenteredLocalBounds;
            }
        }
    }
}
