using UnityEngine;


namespace GestureDetection.Algorithms
{
    public class TestAlgorithm : MonoBehaviour
    {

        public static AlgorithmComponent algorithm;

        // Use this for initialization
        void Start()
        {
            float time = Time.realtimeSinceStartup;
            if (GestureDataManager.Data.Count > 0)
            {
                algorithm = gameObject.AddComponent<AlgorithmComponent>();
                algorithm.Init(GestureDataManager.Data);

                double error = algorithm.ComputeError(GestureDataManager.Data);

                Debug.Log("Time : " + (Time.realtimeSinceStartup - time) + "  Error : " + error);
            }
        }
    }
}