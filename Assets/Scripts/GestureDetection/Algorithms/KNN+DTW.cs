using Accord.MachineLearning.Jambon;
using UnityEngine;

namespace GestureDetection.Algorithms
{
    class KNN : ClassificationAlgorithm
    {
        KNearestNeighbors<double[]> knn;

        public KNN(AlgorithmInput input) : base(input)
        {
        }

        public override string Recognize(double[] input)
        {
            double[] copy = new double[input.Length];
            input.CopyTo(copy, 0);
            //Debug.Log(input.Length);
            //Debug.Log(string.Join(",", input.CustomApply<double,string>(x => x.ToString())));
            return GetClassName(knn.Compute(copy));
        }

        public override double TestEffectiveness(AlgorithmInputData data)
        {
            knn = new KNearestNeighbors<double[]>(3, 2, data.input, data.output, Distance.DynamicTimeWarpDouble);
            int nbFalse = 0;
            for (int i = 0; i < data.input.Length; i++)
            {
                Debug.Log("i ------------------------- " + i);
                //for (int y = 0; y < data.input[i].Length; y++)
                //{
                //    Debug.Log(data.input[i][y]);
                //}
                if (!(knn.Compute(data.input[i]) == data.output[i]))
                {
                    nbFalse++;
                }
            }
            return nbFalse / data.input.Length;
        }

        protected override void Train(AlgorithmInput algorithmInput)
        {
            knn = new KNearestNeighbors(5, algorithmInput.numberClasses, algorithmInput.data.input, algorithmInput.data.output, Distance.DynamicTimeWarpDouble);
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
