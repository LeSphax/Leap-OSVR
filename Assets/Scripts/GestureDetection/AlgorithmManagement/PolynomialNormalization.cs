using Accord.Math;
using Accord.Statistics.Models.Regression.Linear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GestureDetection.Algorithms
{
    class PolynomialNormalization
    {

        private static double[] _integers;
        private int degree;
        private int normalizationLength;

        private static double[] integers
        {
            get
            {
                if (_integers == null)
                {
                    _integers = new double[1000];
                    for (int i = 0; i < 1000; i++)
                    {
                        _integers[i] = i;
                    }
                }
                return _integers;
            }
        }

        public PolynomialNormalization(int degree, int normalizationLength)
        {
            this.degree = degree;
            this.normalizationLength = normalizationLength;
        }

        public double[] Normalize(double[] input)
        {
            PolynomialRegression pr = PolynomialRegression.FromData(degree, integers.Submatrix(0, input.Length - 1), input);
            return pr.Compute(GetInputArray(input.Length, normalizationLength));
        }


        public double[] GetInputArray(int numberPoints, int targetNumberPoints)
        {
            double[] results = new double[targetNumberPoints];
            for (int i = 0; i < targetNumberPoints; i++)
            {
                results[i] = numberPoints * i / targetNumberPoints;
            }
            return results;
        }
    }
}
