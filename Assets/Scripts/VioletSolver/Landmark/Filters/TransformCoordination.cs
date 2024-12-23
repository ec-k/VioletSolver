namespace VioletSolver.Landmarks
{
    public class TransformCoordination : ILandmarkFilter
    {
        public ILandmarks Filter(ILandmarks landmarks)
        {
            for(int i=0;i<landmarks.Count;i++)
            {
                landmarks.Landmarks[i] = new Landmark(
                    -landmarks.Landmarks[i].X,
                    -landmarks.Landmarks[i].Y,
                    -landmarks.Landmarks[i].Z,
                    landmarks.Landmarks[i].Confidence
                    );
            }
            return landmarks;
        }
    }
}
