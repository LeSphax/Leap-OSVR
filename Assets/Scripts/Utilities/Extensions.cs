

using Leap;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Utilities
{
    public static class MyExtensions
    {

        public static void Populate<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }

        public static void Append<T>(this List<T> list, List<T> other)
        {
            list.InsertRange(list.Count, other);
        }


        public static double[] ToDoubleArray(this Vector vector)
        {
            double[] result = new double[3];
            result[0] = vector.x;
            result[1] = vector.y;
            result[2] = vector.z;
            return result;
        }

        public static Vector3 ToVector3(this double[] array)
        {
            Assert.IsTrue(array.Length <= 3);
            Vector3 result;
            result.x = (float)array[0];
            if (array.Length == 1)
            {
                result.y = (float)array[0];
                result.z = (float)array[0];
                return result;
            }
            result.y = (float)array[1];
            if (array.Length == 2)
            {
                result.z = 0;
                return result;
            }
            result.z = (float)array[2];
            return result;
        }

        public static Type[] ValueToArray<Type>(this Type value)
        {
            Type[] result = new Type[1];
            result[0] = value;
            return result;
        }

        public static List<T> Intersect<T>(this List<T> list, List<T> other)
        {
            List<T> result = new List<T>();
            for (int i = 0; i < list.Count; i++)
            {
                T value = list[i];
                if (other.Contains(value))
                {
                    result.Add(value);
                }
            }
            return result;
        }
    }
}

