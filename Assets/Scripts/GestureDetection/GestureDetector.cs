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
        private SerializableListDictionary<int, Gesture> framesToCheck;

        private Gesture currentData;

        public delegate void GestureHandler(Hand hand);
        private GestureHandler newHandlerToSave;
        private string nameNewGesture;

        void Awake()
        {
            Debug.Log(System.Environment.Version);
            registrations = new Dictionary<Gesture, GestureHandler>();
            framesToCheck = new SerializableListDictionary<int, Gesture>();

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
            Debug.Log("SaveGesture");
            // registrations.Add(currentData, newHandlerToSave);
            GestureDataManager.Add(nameNewGesture, currentData);
            //Saving.Save("Test.xml", currentData);
        }

        internal void AddFrame()
        {
            foreach (Parameter parameter in Gesture.parameters)
            {
                currentData[parameter.key].Add(parameter.getParameter(HandModel));
            }
        }

        internal void CheckBeginningGesture()
        {
            //Debug.Log("First : " +Time.timeSinceLevelLoad);
            foreach (Gesture savedGesture in registrations.Keys)
            {
                if (savedGesture.NumberPoints < 10)
                {
                    Debug.Log("Inferior " + savedGesture.NumberPoints);
                }
                if (savedGesture.IsSimilarToBeginning(currentData))
                {
                    int indexPredictedLastKeyFrame = currentData.NumberPoints - 1 + savedGesture.NumberPoints - 1;
                    framesToCheck.Add(indexPredictedLastKeyFrame, savedGesture);
                    Debug.LogWarning("SimilarBeginning");
                }
            }
            //Debug.Log("Second : " +Time.timeSinceLevelLoad);
        }

        internal void CheckSimilarity()
        {

            List<Gesture> list;
            if (framesToCheck.TryGetValue(currentData.NumberPoints, out list))
            {
                //Debug.Log(currentData.NumberPoints+" : " + Time.timeSinceLevelLoad);
                foreach (Gesture gesture in list)
                {
                    Assert.IsTrue(currentData.NumberPoints > 0);
                    Assert.IsTrue(gesture.NumberPoints > 0);
                    if (Gesture.AreGesturesSimilar(gesture, currentData.GetSubGesture(currentData.NumberPoints - gesture.NumberPoints, gesture.NumberPoints)))
                    {
                        stateMachine.handleEvent(new GestureDetectedEvent(gesture));
                        return;
                    }
                }
                //Debug.Log(currentData.NumberPoints + " : " + Time.timeSinceLevelLoad);
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
