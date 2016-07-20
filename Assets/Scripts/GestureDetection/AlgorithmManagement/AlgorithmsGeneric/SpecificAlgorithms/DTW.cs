using Accord.MachineLearning;
using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Utilities;

namespace GestureDetection.Algorithms
{
    public class DTW : UnivariateAlgorithm
    {
        private Dictionary<int, double[]> registers = new Dictionary<int, double[]>();
        private Dictionary<int, double> thresholds = new Dictionary<int, double>();

        public DTW(string parameterName, int parameterIndex, UnivariateAlgorithmInput input) : base(parameterName, parameterIndex, input)
        {
        }

        public override DistanceResult Recognize(double[] input, out List<string> errors)
        {
            errors = new List<string>();
            DistanceResult results = new DistanceResult();
            foreach (int key in registers.Keys)
            {
                double dist = ComputeDistance(input, registers[key]);
                double threshold = thresholds[key];
                if (dist <= threshold)
                {
                    results.Add(key, dist);
                }
                else
                {
                    errors.Add(GestureDataManager.GetClassName(key) + " failed at " + parameterName + parameterIndex + "  : " + dist + " vs " + threshold);
                }
            }
            if (results.Count == 0)
            {
                results.Add(-1, 0);
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
                    List<string> errors;
                    if (!(Recognize(data.input[i], out errors).ContainsKey(data.output[i])))
                    {
                        nbFalse++;
                    }
                    else
                    {
                        nbTrue++;
                    }
                }
            }
            double error = nbFalse / (nbFalse + nbTrue);
            return error;
        }

        public override void Train(UnivariateAlgorithmInput algorithmInput)
        {
            if (registers.Count == 0)
            {
                throw new Exception("Best gestures need to be registered before this method is called");
            }
            CreateThresholds(algorithmInput);
        }

        private Dictionary<int, double> CreateThresholds(UnivariateAlgorithmInput algorithmInput)
        {
            AlgorithmInputData data = algorithmInput.data;
            thresholds = new Dictionary<int, double>();
            for (int i = 0; i < data.input.Length; i++)
            {
                int classIndex = data.output[i];
                if (GestureDataManager.GetClassName(classIndex) != "Nothing")
                {
                    double distance = ComputeDistance(data.input[i], registers[classIndex]);
                    if (!thresholds.ContainsKey(classIndex))
                    {
                        thresholds[classIndex] = distance;
                    }
                    else
                    {
                        thresholds[classIndex] = Math.Max(thresholds[classIndex], distance);
                    }
                }
            }
            List<int> keys = thresholds.Keys.ToList();
            foreach (int key in keys)
            {
                thresholds[key] *= 2;
            }
            return thresholds;
        }

        public double ComputeDistance(double[] first, double[] second)
        {
            return Distance.DynamicTimeWarpDouble(first, second);
        }

        public override double ComputeDistance(Gesture gesture1, Gesture gesture2)
        {
            double[] valuesGesture1 = GetValuesFromGesture(gesture1);
            double[] valuesGesture2 = GetValuesFromGesture(gesture2);
            return ComputeDistance(valuesGesture1, valuesGesture2);
        }

        private double[] GetValuesFromGesture(Gesture gesture)
        {
            return gesture[parameterName].GetNormalizedValues().Transpose()[parameterIndex];
        }

        public override void SetBestGesture(int classNumber, Gesture gesture)
        {
            registers.Add(classNumber, GetValuesFromGesture(gesture));
        }
    }
}
