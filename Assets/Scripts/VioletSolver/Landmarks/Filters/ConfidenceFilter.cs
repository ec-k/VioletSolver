using System;
using System.Collections.Generic;

namespace VioletSolver
{
    public class ConfidenceFilter: ILandmarkFilter
    {
        List<Landmark> _previousLandmarks;
        float _threshold;
        float _valueOnThreshold;

        public ConfidenceFilter(float threshold, float valueOnThreshold) 
        { 
            _previousLandmarks = new List<Landmark>(30);
            _threshold = threshold;
            _valueOnThreshold = valueOnThreshold;
        }

        public ILandmarks Filter(ILandmarks landmarks, float amount)
        {
            for (var i = 0; i < Math.Min(landmarks.Count, _previousLandmarks.Count); i++)
            {
                landmarks.Landmarks[i] = Filter(_previousLandmarks[i], landmarks.Landmarks[i]);
            }

            UpdatePrevLandmarks(landmarks);
                                                
            return landmarks;
        }

        void UpdatePrevLandmarks(ILandmarks landmarks)
        {
            Utils<Landmark>.ExpandList(landmarks.Count, ref _previousLandmarks);
            _previousLandmarks = landmarks.Landmarks;
        }


        Landmark Filter(Landmark prevLandmark, Landmark targetLandmark)
        {
            var amount = CombinedLinear(targetLandmark.Confidence, _threshold, _valueOnThreshold);
            var result = Landmark.Lerp(prevLandmark, targetLandmark, amount);
            return result;
        }

        float CombinedLinear(float x, float x_th, float y_th)
        {
            if (x < 0.2f)
            {
                return 0f;
            }
            else if (x < x_th)
            {
                return x * (y_th / x_th);
            }
            else
            {
                return (1 - y_th) / (1 - x_th) * (x - x_th) + y_th;
            }
        }
    }
}
