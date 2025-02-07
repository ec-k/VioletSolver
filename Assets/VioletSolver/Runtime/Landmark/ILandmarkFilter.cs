namespace VioletSolver.Landmarks
{
    internal interface ILandmarkFilter
    {
        public ILandmarks Filter(ILandmarks landmarks);
    }
}
