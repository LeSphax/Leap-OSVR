using System.Collections.Generic;
using UnityEngine;

namespace GestureDetection.Algorithms
{
    public class AlgorithmComponent: MonoBehaviour
    {

        private ClassificationAlgorithm algorithm;
        public AlgorithmComponent()
        {
            
        }

        public void Init(GestureData gestureData)
        {
            algorithm = new MultivariateAlgorithm<DTW>(gestureData);
        }

        void OnDrawGizmos()
        {
            if (algorithm.gesturesToDraw.Count > 0)
            {
                algorithm.gesturesToDraw[0].DrawGizmos(Color.blue);
            }
        }

        public double ComputeError(GestureData data)
        {
            return algorithm.ComputeError(data);
        }

        public DistanceResult Recognize(Gesture gesture)
        {
            List<string> errors;
            return algorithm.Recognize(gesture, out errors);
        }

        public DistanceResult Recognize(Gesture gesture, out List<string> errors)
        {
            return algorithm.Recognize(gesture,out errors);
        }
    }
    
}
