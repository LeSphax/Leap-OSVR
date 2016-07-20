using Accord.Math;
using GestureDetection.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

namespace GestureDetection.Algorithms
{
    public class MultivariateAlgorithm<Algorithm> : ClassificationAlgorithm where Algorithm : UnivariateAlgorithm
    {

        private class ParametersToAlgorithms : Dictionary<string, UnivariateAlgorithm[]>
        {
        }
        private ParametersToAlgorithms parametersToAlgorithms = new ParametersToAlgorithms();

        private List<UnivariateAlgorithm> _algorithms;
        private List<UnivariateAlgorithm> algorithms
        {
            get
            {
                if (_algorithms == null)
                {
                    _algorithms = new List<UnivariateAlgorithm>();
                    foreach (string key in Gesture.parameterKeys)
                    {
                        for (int i = 0; i < parametersToAlgorithms[key].Length; i++)
                        {
                            _algorithms.Add(parametersToAlgorithms[key][i]);
                        }
                    }
                }
                return _algorithms;
            }
        }

        public MultivariateAlgorithm(GestureData input) : base(input)
        {
        }

        protected override void Train(GestureData gestureData)
        {
            MultivariateAlgorithmInput multivariateInput = new MultivariateAlgorithmInput(gestureData);
            CreateUnivariateAlgorithms(multivariateInput);

            FindBestGestures(gestureData);
            TrainAlgorithms(multivariateInput);
        }

        private void TrainAlgorithms(MultivariateAlgorithmInput multivariateInput)
        {
            foreach (string key in Gesture.parameterKeys)
            {
                for (int i = 0; i < parametersToAlgorithms[key].Length; i++)
                {
                    parametersToAlgorithms[key][i].Train(multivariateInput[key][i]);
                }
            }
        }

        private void FindBestGestures(GestureData gestureData)
        {
            foreach (string key in gestureData.Keys)
            {
                Gesture bestGesture = GetBestGesture(key, gestureData);
                gesturesToDraw.Add(bestGesture);
                foreach (UnivariateAlgorithm algorithm in algorithms)
                {
                   algorithm.SetBestGesture(GestureDataManager.GetClassNumber(key),bestGesture );
                }
            }
        }

        private Gesture GetBestGesture(string className, GestureData gestureData)
        {
            List<Gesture> gestures = gestureData[className];
            int numberGestures = gestures.Count;
            double[] distancesAverage = new double[numberGestures];
            for (int i = 0; i < numberGestures; i++)
            {
                double[] distances = new double[numberGestures - 1];
                for (int y = 0; y < numberGestures; y++)
                {
                    if (i > y)
                        distances[y] = ComputeDistance(gestures[i], gestures[y]);
                    else if (i < y)
                        distances[y - 1] = ComputeDistance(gestures[i], gestures[y]);
                }
                distancesAverage[i] = distances.Average();
            }
            int indexSmallestDistances;
            distancesAverage.Min(out indexSmallestDistances);
            return gestures[indexSmallestDistances];
        }

        private void CreateUnivariateAlgorithms(MultivariateAlgorithmInput multivariateInput)
        {
            foreach (string key in Gesture.parameterKeys)
            {
                UnivariateAlgorithmInput[] value = multivariateInput[key];
                int dimension = value.Length;
                UnivariateAlgorithm[] algorithms = new UnivariateAlgorithm[dimension];
                for (int i = 0; i < dimension; i++)
                {
                    object[] args = { key, i, value[i] };
                    algorithms[i] = (UnivariateAlgorithm)Activator.CreateInstance(typeof(Algorithm), args);
                }
                parametersToAlgorithms[key] = algorithms;
            }
        }

        public override DistanceResult Recognize(Gesture gesture, out List<string> errors)
        {
            errors = new List<string>();
            Dictionary<string, DistanceResult> results = new Dictionary<string, DistanceResult>();

            foreach (string key in Gesture.parameterKeys)
            {
                UnivariateAlgorithm[] univariateAlgorithms = parametersToAlgorithms[key];
                ParameterValues parameterValues = gesture[key];
                double[][] normalizedValues = parameterValues.GetNormalizedValues();
                normalizedValues = normalizedValues.Transpose();
                DistanceResult resultsParameter = null;
                for (int i = 0; i < parameterValues.Dimension; i++)
                {
                    List<string> parameterErrors;
                    DistanceResult tempResult = univariateAlgorithms[i].Recognize(normalizedValues[i], out parameterErrors);
                    errors.Append(parameterErrors);
                    if (resultsParameter == null)
                    {
                        resultsParameter = tempResult;
                    }
                    else
                    {
                        resultsParameter = resultsParameter.Intersect(tempResult);
                    }
                }

                results.Add(key, resultsParameter);
            }
            DistanceResult possibleResults = null;
            List<int> impossibleResults = new List<int>();
            foreach (string key in Gesture.parameterKeys)
            {
                if (possibleResults == null)
                {
                    possibleResults = results[key];
                }
                else
                {
                    List<int> keys = possibleResults.Keys.ToList();
                    foreach (int result in keys)
                    {
                        if (!results[key].ContainsKey(result))
                        {
                            impossibleResults.Add(result);
                        }
                        else
                        {
                            possibleResults[result] = possibleResults[result] + results[key][result];
                        }
                    }
                }
            }
            foreach(int classIndex in impossibleResults)
            {
                possibleResults.Remove(classIndex);
            }
            if (possibleResults.Count == 0)
            {
                possibleResults.Add(-1,0);
            }
            return possibleResults;
        }

        public override double ComputeError(GestureData data)
        {
            MultivariateAlgorithmInput multivariateInput = new MultivariateAlgorithmInput(data);
            List<double> results = new List<double>();
            foreach (string key in Gesture.parameterKeys)
            {
                UnivariateAlgorithmInput[] value = multivariateInput[key];
                int dimension = value.Length;
                for (int i = 0; i < dimension; i++)
                {
                    results.Add(parametersToAlgorithms[key][i].ComputeError(value[i].data));
                }
            }
            return results.ToArray().Max();
        }

        private double ComputeDistance(Gesture gesture1, Gesture gesture2)
        {
            double[] distances = new double[algorithms.Count];
            for(int i =0; i< algorithms.Count; i++)
            {
                distances[i] = algorithms[i].ComputeDistance(gesture1, gesture2);
            }
            double max = distances.Max();
            double average = distances.Average();
            return average + max;
        }


    }

    public abstract class UnivariateAlgorithm
    {
        public string parameterName
        {
            get;
            private set;
        }

        public int parameterIndex
        {
            get;
            private set;
        }

        public UnivariateAlgorithm(string parameterName, int parameterIndex, UnivariateAlgorithmInput input)
        {
            this.parameterName = parameterName;
            this.parameterIndex = parameterIndex;
        }

        public abstract void Train(UnivariateAlgorithmInput input);

        public abstract double ComputeError(AlgorithmInputData input);

        public abstract DistanceResult Recognize(double[] input, out List<string> errors);

        public abstract double ComputeDistance(Gesture gesture1, Gesture gesture2);

        public abstract void SetBestGesture(int v, Gesture gesture);
    }


}
