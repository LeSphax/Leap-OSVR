using System;
using System.Collections.Generic;
using UnityEngine;

namespace GestureDetection.Algorithms
{
    public abstract class ClassificationAlgorithm
    {
        public List<Gesture> gesturesToDraw = new List<Gesture>();

        public ClassificationAlgorithm(GestureData gestureData)
        {
            Train(gestureData);
        }

        protected abstract void Train(GestureData data);

        public abstract double ComputeError(GestureData data);

        public abstract DistanceResult Recognize(Gesture gesture, out List<string> errors);
    }

    public class DistanceResult : Dictionary<int, double>
    {

        public DistanceResult Intersect(DistanceResult other)
        {
            DistanceResult result = new DistanceResult();
            foreach (int key in Keys)
            {
                double value;
                if (other.TryGetValue(key, out value))
                {
                    double averageValues = this[key] + value;
                    result.Add(key, averageValues);
                }
            }
            return result;
        }

        internal int Best()
        {
            int min = -1;
            foreach (int key in Keys)
            {
                if (min == -1)
                {
                    min = key;
                }
                else if (this[key] < this[min])
                {
                    min = key;
                }
            }
            return min;
        }
    }

}
