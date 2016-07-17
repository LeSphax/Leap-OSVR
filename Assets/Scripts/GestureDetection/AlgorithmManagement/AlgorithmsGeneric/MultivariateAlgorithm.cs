using Accord.Math;
using GestureDetection.Algorithms;
using System;
using System.Collections.Generic;
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

        public MultivariateAlgorithm(GestureData input) : base(input)
        {
        }

        protected override void Train(GestureData data)
        {
            MultivariateAlgorithmInput multivariateInput = new MultivariateAlgorithmInput(data);
            foreach (string key in Gesture.parameterKeys)
            {                    
                UnivariateAlgorithmInput[] value = multivariateInput.parametersData[key];
                int dimension = value.Length;
                UnivariateAlgorithm[] algorithms = new UnivariateAlgorithm[dimension];
                for (int i = 0; i < dimension; i++)
                {
                    object[] args = { key, value[i] };
                    algorithms[i] = (UnivariateAlgorithm)Activator.CreateInstance(typeof(Algorithm), args);
                }
                parametersToAlgorithms[key] = algorithms;
            }
        }

        public override List<int> Recognize(Gesture gesture)
        {
            Dictionary<string, List<int>> results = new Dictionary<string, List<int>>();

            foreach (string key in Gesture.parameterKeys)
            {
                UnivariateAlgorithm[] univariateAlgorithms = parametersToAlgorithms[key];
                ParameterValues parameterValues = gesture[key];
                double[][] normalizedValues = parameterValues.GetNormalizedValues();
                List<int> resultsParameter = null;
                for (int i = 0; i < parameterValues.Dimension; i++)
                {
                    List<int> tempResult = univariateAlgorithms[i].Recognize(normalizedValues[i]);
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
            List<int> possibleResults = null;
            foreach (string key in Gesture.parameterKeys)
            {
                if (possibleResults == null)
                {
                    possibleResults = results[key];
                }
                else
                {
                    List<int> impossibleResults = new List<int>();
                    foreach (int result in results[key])
                    {
                        if (!results[key].Contains(result))
                        {
                            impossibleResults.Add(result);
                        }
                    }
                    foreach(int result in impossibleResults)
                    {
                        possibleResults.Remove(result);
                    }
                }
            }
            if (possibleResults.Count == 0)
            {
                possibleResults.Add(-1);
            }
            return possibleResults;
        }

        public override double ComputeError(GestureData data)
        {
            MultivariateAlgorithmInput multivariateInput = new MultivariateAlgorithmInput(data);
            List<double> results = new List<double>();
            foreach (string key in Gesture.parameterKeys)
            {
                UnivariateAlgorithmInput[] value = multivariateInput.parametersData[key];
                int dimension = value.Length;
                UnivariateAlgorithm[] algorithms = new UnivariateAlgorithm[dimension];
                for (int i = 0; i < dimension; i++)
                {
                    results.Add(parametersToAlgorithms[key][i].ComputeError(value[i].data));
                }
            }
            return results.ToArray().Max();
        }
    }

    public abstract class UnivariateAlgorithm
    {
        public string parameterName
        {
            get;
            private set;
        }

        public UnivariateAlgorithm(string parameterName, UnivariateAlgorithmInput input)
        {
            this.parameterName = parameterName;
            Train(input);
        }

        protected abstract void Train(UnivariateAlgorithmInput input);

        public abstract double ComputeError(AlgorithmInputData input);

        public abstract List<int> Recognize(double[] input);
    }

}
