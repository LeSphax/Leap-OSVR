using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestureDetection.Algorithms
{
    public abstract class UniVariateAlgorithm : ClassificationAlgorithm
    {

            public UniVariateAlgorithm(AlgorithmInput input) : base(input)
            {
                classNames = input.classNames;
                Train(input);
            }

            protected override void Train(AlgorithmInput input)
        {

        }

            public abstract double TestEffectiveness(AlgorithmInputData input);

            public abstract string Recognize(double[] input);
        
    }
}
