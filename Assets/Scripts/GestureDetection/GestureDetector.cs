using GestureDetection.Algorithms;
using GestureDetection.StateMachine;
using Leap;
using Leap.Unity;
using SaveManagement;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace GestureDetection
{
    public class GestureDetector : Detector
    {
        private const int MAX_WINDOW_SIZE = 120;
        [Tooltip("The hand model to watch. Set automatically if detector is on a hand.")]
        public IHandModel HandModel = null;
        internal GestureStateMachine stateMachine;

        private Dictionary<string, GestureHandler> registrations = new Dictionary<string, GestureHandler>();

        private Gesture currentData;

        public delegate void GestureHandler(Hand hand);
        private GestureHandler newHandlerToSave;
        private string nameNewGesture;

        public bool OnlyLearning;

        public Dictionary<Gesture, List<string>> subGesturesToVisualize = new Dictionary<Gesture, List<string>>();

        void Awake()
        {


            if (HandModel == null)
            {
                HandModel = gameObject.GetComponentInParent<IHandModel>();
            }
            stateMachine = new GestureStateMachine(this);
        }

        public void AddListener(string name, GestureHandler handler)
        {
            registrations.Add(name, handler);
        }

        void Update()
        {
            //
            float time = Time.realtimeSinceStartup;
            stateMachine.Update();
            Debug.Log("TopUpdate : NumberPoints : " + currentData.NumberPoints + " - UpdateTime : " + (Time.realtimeSinceStartup - time));
        }

        void OnDisable()
        {
            stateMachine.handleEvent(new DisabledEvent());
        }

        internal bool IsThereMovement()
        {
            if (Mathf.Abs(HandModel.GetLeapHand().PalmVelocity.Magnitude) > 0.2f)
            {
                return true;
            }
            return false;
        }

        internal void StartGesture()
        {
            currentData = new Gesture();
        }

        private float distanceSquared(GameObject target)
        {
            Collider targetCollider = target.GetComponent<Collider>();
            Vector3 closestPoint;
            if (targetCollider != null)
            {
                closestPoint = targetCollider.ClosestPointOnBounds(transform.position);
            }
            else
            {
                closestPoint = target.transform.position;
            }
            return (closestPoint - transform.position).sqrMagnitude;
        }

        internal void SaveGesture()
        {
            if (currentData.NumberPoints > 10)
            {
                Debug.Log("SaveGesture " + currentData.NumberPoints);
                // registrations.Add(currentData, newHandlerToSave);
                GestureDataManager.Add(nameNewGesture, currentData);
                //Saving.Save("Test.xml", currentData);
            }
            else
            {
                Debug.LogWarning("Dismissed Data");
            }
        }

        internal void AddFrame()
        {
            foreach (Parameter parameter in Gesture.parameters)
            {
                currentData[parameter.key].Add(parameter.getParameter(HandModel));
            }
        }

        internal void CheckSimilarityWithAll()
        {
            if (!OnlyLearning)
            {
                Gesture subGesture = null;
                int targetNumberPoints = 0;
                // float time = Time.realtimeSinceStartup;
                for (targetNumberPoints = 80; targetNumberPoints <= MAX_WINDOW_SIZE; targetNumberPoints += 10)
                {

                    if (currentData.NumberPoints >= targetNumberPoints)
                    {
                        subGesture = currentData.GetSubGesture(currentData.NumberPoints - targetNumberPoints, targetNumberPoints);

                        List<string> errors;
                        string gestureClass = subGesture.GetGestureClass(TestAlgorithm.algorithm, out errors);
                        subGesturesToVisualize.Add(subGesture, errors);
                        if (gestureClass != "None")
                        {
                            Debug.LogWarning(gestureClass);
                            subGesture = null;
                            stateMachine.handleEvent(new GestureDetectedEvent(gestureClass));
                        }
                    }
                    else
                    {
                        break;
                    }

                }
                if (targetNumberPoints == MAX_WINDOW_SIZE + 10 && subGesture != null)
                {
                    currentData = subGesture;
                }
                //Debug.Log(Time.realtimeSinceStartup - time);
            }

        }


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (ShowGizmos)
            {
                if (currentData != null)
                {
                    currentData.DrawGizmos();
                }
                if (stateMachine != null)
                    stateMachine.DrawGizmos();
            }
        }
#endif

        internal void ExecuteHandler(string className)
        {
            GestureHandler handler;
            if (registrations.TryGetValue(className, out handler))
            {
                handler.Invoke(HandModel.GetLeapHand());
            }
            else
            {
                Debug.LogError("The gest that has been recognised should be in the list " + className);
            }
        }
    }
}
