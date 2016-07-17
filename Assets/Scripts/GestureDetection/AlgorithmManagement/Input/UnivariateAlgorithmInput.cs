using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utilities;

namespace GestureDetection.Algorithms
{
    public struct AlgorithmInputData
    {
        public double[][] input;
        public int[] output;
    }

    public class UnivariateAlgorithmInput
    {
        private List<double[]> inputData = new List<double[]>();
        private List<int> outputData = new List<int>();
        private MultivariateAlgorithmInput parent;

        private bool hasChanged = true;

        private AlgorithmInputData _data;
        public AlgorithmInputData data
        {
            get
            {
                if (hasChanged)
                {
                    _data = CreateData();
                    hasChanged = false;
                }
                return _data;
            }
        }

        private AlgorithmInputData CreateData()
        {
            AlgorithmInputData data;
            data.input = inputData.ToArray();
            data.output = outputData.ToArray();
            return data;
        }

        public UnivariateAlgorithmInput()
        {

        }

        public UnivariateAlgorithmInput(MultivariateAlgorithmInput parent)
        {
            this.parent = parent;
        }

        public void AddData(double[] input, int output)
        {
            inputData.Add(input);
            outputData.Add(output);
            hasChanged = true;
        }
    }

}
