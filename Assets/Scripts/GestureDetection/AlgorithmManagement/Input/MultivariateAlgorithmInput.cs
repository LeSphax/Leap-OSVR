using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using Utilities;

namespace GestureDetection.Algorithms
{

    public class MultivariateAlgorithmInput
    {

        public class ParameterToData : Dictionary<string, UnivariateAlgorithmInput[]>
        {

        }
        public ParameterToData parametersData;

        public int numberClasses
        {
            get
            {
                return GestureDataManager.numberClasses;
            }
        }

        public MultivariateAlgorithmInput(GestureData gestureData)
        {
            parametersData = CreateDictionary();
            InitData(gestureData);
            //foreach (string key in parametersData.Keys)
            //{
            //    Debug.Log(parametersData[key].Apply((algo) => (algo.data.input.Apply(input => input.Print())).Print()).Print());
            //}
        }

        private ParameterToData CreateDictionary()
        {
            ParameterToData result = new ParameterToData();
            foreach (Parameter parameter in Gesture.parameters)
            {
                UnivariateAlgorithmInput[] inputs = new UnivariateAlgorithmInput[parameter.Dimension()];
                for (int i = 0; i < parameter.Dimension(); i++)
                {
                    inputs[i] = new UnivariateAlgorithmInput(this);
                }
                result.Add(parameter.key, inputs);
            }
            Assert.AreEqual(Gesture.parameterKeys.Count, result.Count);
            return result;
        }

        private void InitData(GestureData gestureData)
        {
            Assert.IsTrue(gestureData.Count > 0);
            foreach (string key in gestureData.Keys)
            {
                int index = GestureDataManager.GetClassNumber(key);
                foreach (Gesture gesture in gestureData[key])
                {
                    Assert.IsTrue(gesture.NumberPoints > 0);
                    foreach (ParameterValues parameterValues in gesture.Values)
                    {
                        double[][] normalizedValues = parameterValues.GetNormalizedValues();
                        Assert.IsNotNull(normalizedValues);
                        double[][] separatedDimensions = normalizedValues.Transpose();
                        Assert.IsTrue(separatedDimensions.Length == normalizedValues[0].Length);
                        for (int i = 0; i < parameterValues.Dimension; i++)
                        {
                            Assert.IsNotNull(separatedDimensions[i]);
                            Assert.IsTrue(separatedDimensions[i].Length > 10);
                            parametersData[parameterValues.parameterKey][i].AddData(separatedDimensions[i], index);
                        }

                    }
                }
            }
        }
    }

}
