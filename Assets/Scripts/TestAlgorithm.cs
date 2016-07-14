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
            AlgorithmInput algorithmInput = new AlgorithmInput(GestureDataManager.data);

            float time = Time.realtimeSinceStartup;
            algorithm = new KNN(algorithmInput);

            double error = 0;// = algorithm.TestEffectiveness(algorithmInput.data);

            Debug.Log("Time : " + (Time.realtimeSinceStartup - time) + "  Error : " + error);
        }
    }
}