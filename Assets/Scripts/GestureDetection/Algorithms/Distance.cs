using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GestureDetection.Algorithms
{
    class Distance
    {

        public static double DynamicTimeWarpVector3(Vector3[] table1, Vector3[] table2, Func<Vector3, Vector3, float> diff)
        {
            return DynamicTimeWarp(table1, table2, ((x, y) => diff(x,y)));
        }

        //Copied on wikipedia "Dynamic time warping"
        public static double DynamicTimeWarp<Type>(Type[] table1, Type[] table2, Func<Type,Type,double> diff)
        {
            int sizeGesture1 = table1.Length;
            int sizeGesture2 = table2.Length;
            double[,] DTW = new double[sizeGesture1 + 1, sizeGesture2 + 1];

            for (int i = 1; i <= sizeGesture1; i++)
            {
                DTW[i, 0] = Mathf.Infinity;
            }
            for (int i = 1; i <= sizeGesture2; i++)
            {
                DTW[0, i] = Mathf.Infinity;
            }
            DTW[0, 0] = 0f;

            for (int i = 0; i < sizeGesture1; i++)
            {
                for (int j = 0; j < sizeGesture2; j++)
                {
                    double cost = diff(table1[i], table2[j]);
                    DTW[i + 1, j + 1] = cost + Math.Min(DTW[i, j + 1], Math.Min(DTW[i + 1, j], DTW[i, j]));
                }
            }
            return DTW[sizeGesture1, sizeGesture2];
        }


        public static double DynamicTimeWarpDouble(double[] gesture1, double[] gesture2)
        {
            return DynamicTimeWarp<double>(gesture1, gesture2, ((x, y) => x - y));
        }
    }
}
