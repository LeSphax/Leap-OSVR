using System.Collections.Generic;
using UnityEngine;

namespace GestureDetection.Algorithms
{
    public abstract class ClassificationAlgorithm
    {
        protected Dictionary<int, string> classNames;

        public ClassificationAlgorithm(AlgorithmInput input)
        {
            classNames = input.classNames;
            Train(input);
        }

        public string GetClassName(int index)
        {
            if (index == -1)
            {
                return "None";
            }
            return classNames[index];
        }

        protected abstract void Train(AlgorithmInput input);

        public abstract double TestEffectiveness(AlgorithmInputData input);

        public abstract string Recognize(double[] input);
    }

    public struct AlgorithmInputData
    {
        public double[][] input;
        public int[] output;
    }

    public class AlgorithmInput
    {
        public AlgorithmInputData data;
        public int numberClasses
        {
            get
            {
                return classNames.Count;
            }
        }
        public Dictionary<int, string> classNames
        {
            get;
            private set;
        }

        public AlgorithmInput(GestureData gestureData)
        {
            classNames = InitClassNames(gestureData);
            this.data = CreateAlgorithmInput(gestureData);
        }

        private Dictionary<int,string> InitClassNames(GestureData gestureData)
        {
            Dictionary<int, string> newClassNames = new Dictionary<int, string>();
            int classIndex = 0;
            foreach (string key in gestureData.Keys)
            {
                newClassNames.Add(classIndex, key);
                classIndex++;
            }
            return newClassNames;
        }

        private AlgorithmInputData CreateAlgorithmInput(GestureData gestureData)
        {
            AlgorithmInputData input;
            input.input = new double[gestureData.GetNumberGestures()][];
            input.output = new int[gestureData.GetNumberGestures()];

            int classNumber = 0;
            int startIndex = 0;
            for (int x = 0; x < classNames.Count; x++)
            {
                string key = classNames[x];
                for (int i = startIndex; i < startIndex + gestureData[key].Count; i++)
                {
                    input.output[i] = classNumber;
                    Gesture gesture = gestureData[key][i - startIndex];
                    input.input[i] = GestureToData(gesture);
                }
                startIndex += gestureData[key].Count;
                classNumber++;
            }
            return input;
        }

        public static double[] GestureToData(Gesture gesture)
        {
            Vector3[] list = Gesture.parameters[0].NormalizeValues(gesture[Gesture.PalmPositions].ToArray());
            double[] result = new double[list.Length];
            for (int y = 0; y < list.Length; y++)
            {
                result[y] = list[y].z;
            }
            return result;
        }
    }

    
}
