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
    class SVM : ClassificationAlgorithm
    {
        MulticlassSupportVectorLearning teacher;
        MulticlassSupportVectorMachine svm;
        PolynomialNormalization normalizer = new PolynomialNormalization(100, NUMBER_POINTS);

        private const int NUMBER_POINTS = 100;

        public SVM(AlgorithmInput input) : base(input)
        {
        }

        public override string Recognize(double[] input)
        {
            input = normalizer.Normalize(input);
            return GetClassName(svm.Compute(input));
        }

        public override double TestEffectiveness(AlgorithmInputData data)
        {
            data = NormalizeInputs(data);
            return teacher.ComputeError(data.input, data.output);
        }

        protected override void Train(AlgorithmInput algorithmInput)
        {
            AlgorithmInputData data = NormalizeInputs(algorithmInput.data);

            IKernel kernel = new Polynomial(10);
            svm = new MulticlassSupportVectorMachine(NUMBER_POINTS , kernel, algorithmInput.numberClasses);

            teacher = new MulticlassSupportVectorLearning(svm, data.input, data.output);

            teacher.Algorithm = (m_svm, classInputs, classOutputs, i, j) =>
    new SequentialMinimalOptimization(m_svm, classInputs, classOutputs);

            teacher.Run();
        }

        private AlgorithmInputData NormalizeInputs(AlgorithmInputData inputData)
        {   
            inputData.input = inputData.input.CustomApply<double[],double[]>(normalizer.Normalize);
            return inputData;
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
