using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Distributions;
using Accord.Statistics.Kernels;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GestureDetection.Algorithms
{
    class MarkivClassifier : Algorithm
    {
        
        public MarkivClassifier()
        {

        }


        public string Recognize(AlgorithmInputData input)
        {
            throw new NotImplementedException();
        }

        public float TestEffectiveness(AlgorithmInputData input)
        {
            throw new NotImplementedException();
        }

        public double Train(AlgorithmInputData input)
        {

            //// Nested models will have two states each
            //int[] states = new int[] { 2, 2 };

            //// Creates a new Hidden Markov Model Classifier with the given parameters
            //HiddenMarkovClassifier<IMultivariateDistribution> classifier = new HiddenMarkovClassifier<IMultivariateDistribution>(async,);

            //// Create a new learning algorithm to train the sequence classifier
            //var teacher = new HiddenMarkovClassifierLearning<IMultivariateDistribution>(classifier,

            //// Train each model until the log-likelihood changes less than 0.001
            //modelIndex => new BaumWelchLearning<IMultivariateDistribution>(classifier.Models[modelIndex])
            //{
            //    Tolerance = 0.001,
            //    Iterations = 0
            //});

            //// Train the sequence classifier using the algorithm
            //double likelihood = teacher.Run(input.data, input.classifications);

            //int[] answers = input.data.Apply(classifier.Compute);

            //Debug.Log(likelihood);
            return 0;
        }

        private AlgorithmInputData NormalizeInputs(AlgorithmInputData input)
        {
            PolynomialNormalization normalizer = new PolynomialNormalization(100,50);
            input.data = input.data.CustomApply<double[],double[]>(normalizer.Normalize);
            Debug.Log(input.data[0].Length);
            return input;
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
