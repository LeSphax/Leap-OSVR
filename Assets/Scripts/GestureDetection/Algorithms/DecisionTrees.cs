using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math;
using GestureDetection.Algorithms;
using System.Collections.Generic;
using UnityEngine;
using System;

//class DecisionTrees : Algorithm
//{

//    public string Recognize(AlgorithmInputData input)
//    {
//        throw new NotImplementedException();
//    }

//    public float TestEffectiveness(AlgorithmInputData input)
//    {
//        throw new NotImplementedException();
//    }

//    //Il faut spécifier les variables à l'avance sur les arbres et les filer à l'algo. Ca à pas l'air d'être mon type de problème
//    public void Train(AlgorithmInputData input)
//    {
//        List<DecisionVariable> decisions = new List<DecisionVariable>
//                        {
//            new DecisionVariable("Load", DecisionVariableKind.Discrete),
//            new DecisionVariable("Random", DecisionVariableKind.Discrete),
//                        };


//        DecisionTree tree = new DecisionTree(decisions, 50);
//        C45Learning teacher = new C45Learning(tree);

//        // The C4.5 algorithm expects the class labels to
//        // range from 0 to k, so we convert -1 to be zero:
//        //
//        //outputs = outputs.CustomApply(x => x < 0 ? 0 : x);
//        double error = teacher.Run(input.data,input.classifications);
//        // Classify the samples using the model
//        Debug.Log(error);

//        int[] answers = input.data.CustomApply<double[],int>(tree.Compute);
//        for(int i =0; i<answers.Length; i++)
//        {
//            Debug.Log(answers[i] == input.classifications[i]);
//        }
//    }
//}
