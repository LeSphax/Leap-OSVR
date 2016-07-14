using GestureDetection.Algorithms;
using UnityEngine;


namespace GestureDetection.Algorithms
{
    public class TestAlgorithm : MonoBehaviour
    {

        public static Algorithm algorithm;

        // Use this for initialization
        void Start()
        {
            AlgorithmInputData input = AlgorithmInputData.ConvertGestureData(GestureDataManager.data);

            float time = Time.realtimeSinceStartup;
            algorithm = new SVM();

            double error = algorithm.Train(input);

            Debug.Log("Time : " + (Time.realtimeSinceStartup - time) + "  Error : " + error);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}