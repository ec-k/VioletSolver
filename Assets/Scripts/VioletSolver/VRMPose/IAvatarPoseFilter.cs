namespace VioletSolver
{
    public interface IAvatarPoseFilter
    {
        public AvatarPoseData Filter(AvatarPoseData boneRotations, float amount);
    }
}
