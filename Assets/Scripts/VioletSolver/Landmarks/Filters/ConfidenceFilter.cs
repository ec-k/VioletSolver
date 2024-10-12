using System;
using System.Collections.Generic;

namespace VioletSolver
{
    public class ConfidenceFilter: ILandmarkFilter
    {
        List<Landmark> _previousLandmarks;

        public ConfidenceFilter() 
        { 
            _previousLandmarks = new List<Landmark>(30);
        }

        public ILandmarks Filter(ILandmarks landmarks, float amount)
        {
            for (var i = 0; i < Math.Min(landmarks.Count, _previousLandmarks.Count); i++)
            {
                landmarks.Landmarks[i] = Filter(_previousLandmarks[i], landmarks.Landmarks[i], amount);
            }

            UpdatePrevLandmarks(landmarks);     // CAUTION: This is a side effect. We have to display this function has side effect
                                                //          or move side effect out of this function.
                                                //          But maybe,  we don't have to consider it because effect range of the side effect is only this class.
                                                //          There is one option that this function have one argument to save prev landmarks or not.
            return landmarks;
        }

        void UpdatePrevLandmarks(ILandmarks landmarks)
        {
            Utils<Landmark>.ExpandList(landmarks.Count, ref _previousLandmarks);
            _previousLandmarks = landmarks.Landmarks;
        }


        Landmark Filter(Landmark prev_landmark, Landmark target_landmark, float coefficient)
        {
            var _amount = Math.Clamp(coefficient, 0, 1) * target_landmark.Confidence;
            var result = Landmark.Lerp(prev_landmark, target_landmark, _amount);
            return result;
        }
    }
}
