using Accord.Math;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace GestureDetection.Algorithms
{

    public class MultivariateAlgorithmInput : Dictionary<string, UnivariateAlgorithmInput[]>
    {

        public int numberClasses
        {
            get
            {
                return GestureDataManager.numberClasses;
            }
        }

        public MultivariateAlgorithmInput(GestureData gestureData)
        {
            CreateDictionary();
            InitData(gestureData);
            //foreach (string key in parametersData.Keys)
            //{
            //    Debug.Log(parametersData[key].Apply((algo) => (algo.data.input.Apply(input => input.Print())).Print()).Print());
            //}
        }

        private void CreateDictionary()
        {
            foreach (Parameter parameter in Gesture.parameters)
            {
                UnivariateAlgorithmInput[] inputs = new UnivariateAlgorithmInput[parameter.Dimension()];
                for (int i = 0; i < parameter.Dimension(); i++)
                {
                    inputs[i] = new UnivariateAlgorithmInput();
                }
                Add(parameter.key, inputs);
            }
            Assert.AreEqual(Gesture.parameterKeys.Count, Count);
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
                            this[parameterValues.parameterKey][i].AddData(separatedDimensions[i], index);
                        }

                    }
                }
            }
        }
    }

}
