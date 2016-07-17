using GestureDetection.Algorithms;
using UnityEngine;


namespace GestureDetection.Algorithms
{
    public class TestAlgorithm : MonoBehaviour
    {

        public static ClassificationAlgorithm algorithm;

        // Use this for initialization
        void Start()
        {
            float time = Time.realtimeSinceStartup;
            if (GestureDataManager.Data.Count > 0)
            {
                algorithm = new MultivariateAlgorithm<DTW>(GestureDataManager.Data);

                double error = algorithm.ComputeError(GestureDataManager.Data);

                Debug.Log("Time : " + (Time.realtimeSinceStartup - time) + "  Error : " + error);
            }
        }
    }
}