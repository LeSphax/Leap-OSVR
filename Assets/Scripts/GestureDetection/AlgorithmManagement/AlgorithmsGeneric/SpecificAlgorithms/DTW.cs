using Accord.MachineLearning;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GestureDetection.Algorithms
{
    public class DTW : UnivariateAlgorithm
    {
        private Dictionary<int, double[]> registers = new Dictionary<int, double[]>();

        public DTW(string parameterName, UnivariateAlgorithmInput input) : base(parameterName, input)
        {
        }

        public override List<int> Recognize(double[] input)
        {
            List<int> results = new List<int>();
            foreach (int key in registers.Keys)
            {
                double dist = Distance.DynamicTimeWarpDouble(input, registers[key]);
                //Debug.Log(parameterName +"   " + dist);
                //Debug.Log(dist);
                if (dist < Gesture.GetParameter(parameterName).threshold)
                {
                    results.Add(key);
                }
                else
                {
                    Debug.Log(GestureDataManager.GetClassName(key) + " failed at " + parameterName + "  : " + dist);
                }
            }
            if (results.Count == 0)
            {
                results.Add(-1);
            }
            return results;
        }


        public override double ComputeError(AlgorithmInputData data)
        {
            Assert.IsTrue(data.input.Length == data.output.Length && data.input.Length > 0, "The length should be equal - input : " + data.input.Print() + " output : " + data.output.Print());
            Assert.IsTrue(data.input[0].Length > 10, "The data is too short :" + data.input[0].Print());
            double nbFalse = 0;
            double nbTrue = 0;

            for (int i = 0; i < data.input.Length; i++)
            {
                if (GestureDataManager.GetClassName(data.output[i]) != "Nothing")
                {
                    if (!(Recognize(data.input[i]).Contains(data.output[i])))
                    {
                        nbFalse++;
                    }
                    else
                    {
                        nbTrue++;
                    }
                }
            }
            return nbFalse / (nbFalse + nbTrue);
        }

        protected override void Train(UnivariateAlgorithmInput algorithmInput)
        {
            foreach (int classification in GestureDataManager.classesMap.Reverse.Keys)
            {
                for (int i = 0; i < algorithmInput.data.output.Length; i++)
                {
                    if (algorithmInput.data.output[i] == classification && GestureDataManager.GetClassName(classification) != "Nothing")
                    {
                        registers.Add(classification, algorithmInput.data.input[i]);
                        break;
                    }
                }
            }
        }

        

    }
}
