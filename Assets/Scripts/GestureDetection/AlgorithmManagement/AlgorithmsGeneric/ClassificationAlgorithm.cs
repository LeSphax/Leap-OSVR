using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace GestureDetection.Algorithms
{
    public abstract class ClassificationAlgorithm
    {

        public ClassificationAlgorithm(GestureData gestureData)
        {
            Train(gestureData);
        }

        protected abstract void Train(GestureData data);

        public abstract double ComputeError(GestureData data);

        public abstract List<int> Recognize(Gesture gesture);
    }
    
}
