using Accord.Math;
using Accord.Math.Transforms;
using Accord.Statistics.Models.Regression.Linear;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace GestureDetection.Algorithms
{
    public interface Algorithm
    {

        void Train(AlgorithmInputData input);

        double TestEffectiveness(AlgorithmInputData input);

        string Recognize(double[] input);
    }

    public struct AlgorithmInputData
    {
        public double[][] data;
        public int[] classifications;
        public int numberClasses;

        public static AlgorithmInputData ConvertGestureData(GestureData gestureData)
        {
            AlgorithmInputData result;
            result.data = new double[gestureData.GetNumberGestures()][];
            result.classifications = new int[gestureData.GetNumberGestures()];
            result.numberClasses = gestureData.Count;


            int classNumber = 0;
            int startIndex = 0;
            foreach (string key in gestureData.Keys)
            {
                for (int i = startIndex; i < startIndex + gestureData[key].Count; i++)
                {
                    result.classifications[i] = classNumber;
                    List<Vector3> list = gestureData[key][i - startIndex][Gesture.PalmPositions];
                    result.data[i] = new double[list.Count];
                    for (int y = 0; y < list.Count; y++)
                    {
                        result.data[i][y] = list[i].z;
                    }
                }
                startIndex += gestureData[key].Count;
                classNumber++;
            }
            return result;
        }
    }

    
}
