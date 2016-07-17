using GenericStateMachine;
using System;
using UnityEngine;

namespace GestureDetection.StateMachine
{
    internal abstract class GestureEvent
    {

    }
    internal class MovementDetectedEvent : GestureEvent
    {

    }
    internal class NoMovementEvent : GestureEvent
    {

    }
    internal class EndTimerEvent : GestureEvent
    {

    }
    internal class DisabledEvent : GestureEvent
    {

    }
    internal class RegistrationEvent: GestureEvent
    {

    }

    internal class GestureDetectedEvent : GestureEvent
    {
        internal Gesture gesture;

        internal GestureDetectedEvent(Gesture gesture)
        {
            this.gesture = gesture;
        }
    }

    internal class GestureStateMachine : StateMachine<GestureStateMachine, GestureEvent>
    {
        internal GestureDetector detector;

        internal GestureStateMachine(GestureDetector detector)
        {
            this.detector = detector;
            InitFirstState();
        }

        protected override void Init()
        {
        }


        internal override State<GestureStateMachine, GestureEvent> DefineFirst()
        {
            return new DetectingGesturesState(this);
        }

        internal void Update()
        {
            GetCurrentState().Update();
        }

        private GestureState GetCurrentState()
        {
            if (typeof(GestureState).IsAssignableFrom(current.GetType()))
            {
                return (GestureState)current;

            }
            else
            {
                throw new Exception("The current state should be of type GestureState not : " + current.GetType());
            }
        }

        internal bool IsThereMovement()
        {
            return detector.IsThereMovement();
        }

        internal void DrawGizmos()
        {
            GetCurrentState().DrawGizmos();
        }
    }

    internal abstract class GestureState : State<GestureStateMachine, GestureEvent>
    {
        internal GestureState(GestureStateMachine stateMachine) : base(stateMachine)
        {
        }

        internal abstract void Update();
        internal abstract void DrawGizmos();

        protected static void DrawGizmoStringTopRight(string text, Color color)
        {
            CustomGizmos.drawUIString(text, new Vector3(Screen.width - 100, Screen.height - 100, 0), color);
        }
    }

    internal class DetectingGesturesState : GestureState
    {
        internal DetectingGesturesState(GestureStateMachine stateMachine) : base(stateMachine)
        {
            new GestureDetectedTransition(this);
            new RegistrationDemandTransition(this);
            new DisabledTransitionNoSave(this);
        }

        public override void enter()
        {
            stateMachine.detector.StartGesture();
        }

        internal override void Update()
        {
            stateMachine.detector.AddFrame();
            stateMachine.detector.CheckSimilarityWithAll();
        }

        internal override void DrawGizmos()
        {
            DrawGizmoStringTopRight("Detecting", Color.green);
        }


    }

    internal class WaitingForMovementState : GestureState
    {
        internal WaitingForMovementState(GestureStateMachine stateMachine) : base(stateMachine)
        {
            new FirstMovementDetectedTransition(this);
            new IdleDisabledTransition(this);
        }

        internal override void Update()
        {
            if (stateMachine.IsThereMovement())
            {
                stateMachine.handleEvent(new MovementDetectedEvent());
            }
        }

        internal override void DrawGizmos()
        {
            DrawGizmoStringTopRight("Waiting", Color.red);
        }


    }

    internal class PerformingGestureState : GestureState
    {
        internal PerformingGestureState(GestureStateMachine stateMachine) : base(stateMachine)
        {
            new NoMovementTransition(this);
            new DisabledTransition(this);
        }

        internal override void Update()
        {
            stateMachine.detector.AddFrame();
            if (!stateMachine.IsThereMovement())
            {
                stateMachine.handleEvent(new NoMovementEvent());
            }
        }
        internal override void DrawGizmos()
        {
            DrawGizmoStringTopRight("Performing", Color.green);
        }
    }

    internal class CheckingEndGestureState : GestureState
    {

        private float noMovementTime;
        private const float END_GESTURE_TIME = 0.1f;

        internal CheckingEndGestureState(GestureStateMachine stateMachine) : base(stateMachine)
        {
            new MovementDetectedTransition(this);
            new EndTimerTransition(this);
            new DisabledTransition(this);
            noMovementTime = Time.realtimeSinceStartup;
        }

        internal override void Update()
        {
            stateMachine.detector.AddFrame();
            if (stateMachine.IsThereMovement())
            {
                stateMachine.handleEvent(new MovementDetectedEvent());
            }
            else if (Time.realtimeSinceStartup - noMovementTime > END_GESTURE_TIME)
            {
                stateMachine.handleEvent(new EndTimerEvent());
            }
        }
        internal override void DrawGizmos()
        {
            DrawGizmoStringTopRight("CheckingEnd", Color.blue);
        }
    }



    internal abstract class GestureTransition<Event> : Transition<GestureStateMachine, GestureEvent, Event> where Event : GestureEvent
    {
        internal GestureTransition(State<GestureStateMachine, GestureEvent> myState) : base(myState)
        {
        }
    }

    internal class FirstMovementDetectedTransition : GestureTransition<MovementDetectedEvent>
    {
        internal FirstMovementDetectedTransition(State<GestureStateMachine, GestureEvent> myState) : base(myState)
        {
        }
        public override void action(MovementDetectedEvent evt)
        {
            myState.stateMachine.detector.StartGesture();
        }

        public override State<GestureStateMachine, GestureEvent> goTo()
        {
            return new PerformingGestureState(myState.stateMachine);
        }
    }

    internal class MovementDetectedTransition : GestureTransition<MovementDetectedEvent>
    {
        internal MovementDetectedTransition(State<GestureStateMachine, GestureEvent> myState) : base(myState)
        {
        }

        public override State<GestureStateMachine, GestureEvent> goTo()
        {
            return new PerformingGestureState(myState.stateMachine);
        }
    }

    internal class NoMovementTransition : GestureTransition<NoMovementEvent>
    {
        internal NoMovementTransition(State<GestureStateMachine, GestureEvent> myState) : base(myState)
        {
        }

        public override State<GestureStateMachine, GestureEvent> goTo()
        {
            return new CheckingEndGestureState(myState.stateMachine);
        }
    }

    internal class IdleDisabledTransition : GestureTransition<DisabledEvent>
    {
        internal IdleDisabledTransition(State<GestureStateMachine, GestureEvent> myState) : base(myState)
        {
        }

        public override State<GestureStateMachine, GestureEvent> goTo()
        {
            return myState;
        }
    }

    internal class DisabledTransition : GestureTransition<DisabledEvent>
    {
        internal DisabledTransition(State<GestureStateMachine, GestureEvent> myState) : base(myState)
        {
        }

        public override void action(DisabledEvent evt)
        {
            myState.stateMachine.detector.SaveGesture();
        }

        public override State<GestureStateMachine, GestureEvent> goTo()
        {
            return new DetectingGesturesState(myState.stateMachine);
        }
    }

    internal class DisabledTransitionNoSave : GestureTransition<DisabledEvent>
    {
        internal DisabledTransitionNoSave(State<GestureStateMachine, GestureEvent> myState) : base(myState)
        {
        }

        public override State<GestureStateMachine, GestureEvent> goTo()
        {
            return new DetectingGesturesState(myState.stateMachine);
        }
    }

    internal class EndTimerTransition : GestureTransition<EndTimerEvent>
    {
        internal EndTimerTransition(State<GestureStateMachine, GestureEvent> myState) : base(myState)
        {
        }

        public override void action(EndTimerEvent evt)
        {
            myState.stateMachine.detector.SaveGesture();
        }

        public override State<GestureStateMachine, GestureEvent> goTo()
        {
            return new DetectingGesturesState(myState.stateMachine);
        }
    }

    internal class RegistrationDemandTransition : GestureTransition<RegistrationEvent>
    {
        internal RegistrationDemandTransition(State<GestureStateMachine, GestureEvent> myState) : base(myState)
        {
        }

        public override State<GestureStateMachine, GestureEvent> goTo()
        {
            return new WaitingForMovementState(myState.stateMachine);
        }
    }

    internal class GestureDetectedTransition : GestureTransition<GestureDetectedEvent>
    {
        internal GestureDetectedTransition(State<GestureStateMachine, GestureEvent> myState) : base(myState)
        {
        }

        public override void action(GestureDetectedEvent evt)
        {
            myState.stateMachine.detector.ExecuteHandler(evt.gesture);
        }

        public override State<GestureStateMachine, GestureEvent> goTo()
        {
            return new DetectingGesturesState(myState.stateMachine);
        }
    }

}