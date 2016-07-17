using GestureDetection.Algorithms;
using GestureDetection.StateMachine;
using Leap;
using Leap.Unity;
using SaveManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GestureDetection
{
    public class GestureDetector : Detector
    {
        [Tooltip("The hand model to watch. Set automatically if detector is on a hand.")]
        public IHandModel HandModel = null;
        internal GestureStateMachine stateMachine;

        private Dictionary<Gesture, GestureHandler> registrations;

        private Gesture currentData;

        public delegate void GestureHandler(Hand hand);
        private GestureHandler newHandlerToSave;
        private string nameNewGesture;

        public bool OnlyLearning;

        void Awake()
        {
            registrations = new Dictionary<Gesture, GestureHandler>();


            if (HandModel == null)
            {
                HandModel = gameObject.GetComponentInParent<IHandModel>();
            }
            stateMachine = new GestureStateMachine(this);
        }

        public void AddGesture(string name, GestureHandler handler)
        {
            nameNewGesture = name;
            newHandlerToSave = handler;
            stateMachine.handleEvent(new RegistrationEvent());
        }

        void Update()
        {
            // Debug.Log(currentData.NumberPoints + " : " + Time.realtimeSinceStartup);
            stateMachine.Update();
            // Debug.Log(currentData.NumberPoints + " : " + Time.realtimeSinceStartup);
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
                Debug.Log("SaveGesture " +currentData.NumberPoints);
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
                if (currentData.NumberPoints >= 50)
                {
                    string gestureClass = currentData.GetSubGesture(currentData.NumberPoints - 50, 50).GetGestureClass(TestAlgorithm.algorithm);
                    if (gestureClass != "None")
                    {
                        Debug.Log(gestureClass);
                        StartGesture();
                    }
                    else
                    {
                       // Debug.Log("None");
                    }
                }
            }
               
        }


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (ShowGizmos)
            {
                if (registrations != null && registrations.Count > 0 && currentData != null)
                {
                    foreach (Gesture gesture in registrations.Keys)
                    {
                        gesture.DrawGizmos(Color.blue);
                        break;
                    }
                }
                if (currentData != null)
                {
                    currentData.DrawGizmos();
                }
                if (stateMachine != null)
                    stateMachine.DrawGizmos();
            }
        }
#endif

        internal void ExecuteHandler(Gesture gesture)
        {
            GestureHandler handler;
            if (registrations.TryGetValue(gesture, out handler))
            {
                handler.Invoke(HandModel.GetLeapHand());
            }
            else
            {
                Debug.LogError("The gest that has been recognised should be in the list " + gesture);
            }
        }
    }
}
