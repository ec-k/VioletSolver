using System;

namespace VioletSolver
{
    public class ConfidenceFilter: ILandmarkFilter
    {
        ILandmarks _previousLandmarks;

        void UpdatePrevLandmarks(ILandmarks landmarks)
        {
            _previousLandmarks = landmarks;
        }

        public ILandmarks Filter(ILandmarks landmarks, float amount)
        {
            for (var i = 0; i < landmarks.Count; i++)
            {
                FilterOnOneLandmark(_previousLandmarks.Landmarks[i], landmarks.Landmarks[i], amount);
            }

            UpdatePrevLandmarks(landmarks);     // CAUTION: This is a side effect. We have to display this function has side effect
                                                //          or move side effect out of this function.
                                                //          But maybe,  we don't have to consider it because effect range of the side effect is only this class.
                                                //          There is one option that this function have one argument to save prev landmarks or not.
            return landmarks;
        }

        Landmark FilterOnOneLandmark(Landmark prev_landmark, Landmark target_landmark, float amount)
        {
            var _amount = Math.Clamp(amount, 0, 1) * target_landmark.Confidence;
            var result = Landmark.Lerp(prev_landmark, target_landmark, _amount);
            return result;
        }
    }
}
