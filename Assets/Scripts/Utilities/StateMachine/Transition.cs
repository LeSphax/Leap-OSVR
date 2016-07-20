using System;
using System.Collections.Generic;

namespace GenericStateMachine
{
    /**
     * As each Transition is mapped to a different EventSubType, it would not be possible to create a Dictionary with different types of Transitions without this interface.
     **/
    internal interface Transition<StateMachineType, EventType> where StateMachineType : StateMachine<StateMachineType, EventType>
    {
        bool PassTransition(EventType evt, out State<StateMachineType, EventType> state);
    }

    /**
     * When it is created, a Transition automatically registers onto its parent State.
     * When a Transition is fired, we first check the guard.
     * If it returns true, then the action is performed and the StateMachine pass go to the State returned by the goTo method.
     **/
    internal abstract class Transition<StateMachineType, EventType, EventSubType> : Transition<StateMachineType, EventType> where StateMachineType : StateMachine<StateMachineType, EventType> where EventSubType : EventType
    {

        public virtual bool guard(EventSubType evt) { return true; }
        public virtual void action(EventSubType evt) { }
        public abstract State<StateMachineType, EventType> goTo();

        public bool PassTransition(EventType evt, out State<StateMachineType, EventType> state)
        {
            if (evt is EventSubType)
            {
                EventSubType realEvent = (EventSubType)evt;
                if (guard(realEvent))
                {
                    action(realEvent);
                    state = goTo();
                    return true;
                }
            }
            else
            {
                throw new Exception("This transition only accepts events of type : " + typeof(EventSubType));
            }
            state = null;
            return false;
        }

        protected State<StateMachineType, EventType> myState;

        internal Transition(State<StateMachineType, EventType> myState)
        {
            this.myState = myState;

            AddTransitionToStateDictionary(myState);

        }

        private void AddTransitionToStateDictionary(State<StateMachineType, EventType> myState)
        {
            Transition<StateMachineType, EventType, EventSubType> t = this;
            List<Transition<StateMachineType, EventType>> ts;
            myState.transitionsPerType.TryGetValue(typeof(EventSubType), out ts);
            if (ts == null)
            {
                ts = new List<Transition<StateMachineType, EventType>>();
                ts.Add(t);
                myState.transitionsPerType.Add(typeof(EventSubType), ts);
            }
            else
            {
                ts.Add(t);
            }
        }
    }

}

