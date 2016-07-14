using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GestureDetection.Algorithms
{
    class KNN : Algorithm
    {
        KNearestNeighbors knn;
        public KNN()
        {

        }


        public string Recognize(double[] input)
        {
            return GestureDataManager.GetKey(knn.Compute(input));
        }

        public double TestEffectiveness(AlgorithmInputData input)
        {
            int nbFalse = 0;
            for (int i = 0; i < input.data.Length; i++)
            {
                if (!(knn.Compute(input.data[i]) == input.classifications[i]))
                {
                    nbFalse++;
                }
            }
            return nbFalse / input.data.Length;
        }

        public void Train(AlgorithmInputData input)
        {
            knn = new KNearestNeighbors(5, input.numberClasses, input.data, input.classifications, Distance.DynamicTimeWarpDouble);
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
