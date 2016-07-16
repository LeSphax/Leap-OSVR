using Accord.MachineLearning;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GestureDetection.Algorithms
{
    class DTW : ClassificationAlgorithm
    {
        private Dictionary<int, double[]> registers = new Dictionary<int, double[]>();

        public DTW(AlgorithmInput input) : base(input)
        {
        }

        public override string Recognize(double[] input)
        {
            double min = Mathf.Infinity;
            string result = "None";
            foreach (int key in registers.Keys)
            {
                double dist = Distance.DynamicTimeWarpDouble(input, registers[key]);
                
                if (dist < 0.2 && dist <min)
                {
                    Debug.Log(GetClassName(key) + "   " + dist);
                    min = dist;
                    result = GetClassName(key);
                }
            }
            //Debug.Log(result);
            return result;
        }

        public override double TestEffectiveness(AlgorithmInputData data)
        {
            double nbFalse = 0;
            double nbTrue = 0;
            for (int i = 0; i < data.input.Length; i++)
            {
                if (GetClassName(data.output[i]) != "Nothing")
                {
                    Debug.Log(Recognize(data.input[i]));
                    if (!(Recognize(data.input[i]) == GetClassName(data.output[i])))
                    {
                        nbFalse++;
                    }
                    else
                    {
                        nbTrue++;
                    }
                }
            }
            return nbFalse / (nbFalse +nbTrue);
        }

        protected override void Train(AlgorithmInput algorithmInput)
        {
            foreach(int classification in algorithmInput.classNames.Keys)
            {
                for(int i=0; i < algorithmInput.data.output.Length; i++)
                {
                    if (algorithmInput.data.output[i] == classification && GetClassName(classification)!="Nothing")
                    {
                        registers.Add(classification, algorithmInput.data.input[i]);
                        break;
                    }
                }
            }
        }

        // Create a L2-regularized L2-loss optimization algorithm for
        // the dual form of the learning problem. This is *exactly* the
        // same method used by LIBLINEAR when specifying -s 1 in the 
        // command line (i.e. L2R_L2LOSS_SVC_DUAL).
        //
        // KernelSupportVectorMachine ksvm = new KernelSupportVectorMachine(new DynamicTimeWarping(20),2);
        //new SupportVectorReduction(ksvm);
        //// Teach the vector machine
        //double error = teacher.Run();

        //// Classify the samples using the model
        //int[] answers = inputs.Apply(svm.Compute).Apply(System.Math.Sign);

    }
}
