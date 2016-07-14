using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Assertions;

namespace GenericStateMachine
{
    /**
     * For each State in the machine, a new class is created.
     * States each contain a dictonary matching the class of an Event to a Transition.
     **/
    internal abstract class State<StateMachineType, EventType> where StateMachineType : StateMachine<StateMachineType, EventType>
    {
        internal StateMachineType stateMachine;

        /**
         * The dictionary mapping Event classes to Transitions.
         * If a key of type A and a key of type B : A are inserted in the dictionary, two keys will be inserted.
         **/
        internal Dictionary<Type, List<Transition<StateMachineType, EventType>>> transitionsPerType = new Dictionary<Type, List<Transition<StateMachineType, EventType>>>(); // with static type checking

        internal State(StateMachineType stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        //Called when this State becomes the current State
        public virtual void enter() { }

        //Called when this State stops being the current State
        public virtual void leave() { }

        /**
         * Fire the transition correponding to the class of the Event.
         * It first checks if the class of the Event is in its dictionary, then it goes up the tree to its parent classes.
         * 
         * Exemple : If the dictionary contains the corresponding keys and values (A,T1) and (B : A,T2). 
         * When it receive an event of type B the transition T2 will be called and when it receives an event of type A T1 will be called.
         * 
         * A State should never receive Events that aren't registered or an error will be thrown.
         * If you want a State to accept an Event, you have to register a Transition even if it doesn't do anything.
         **/
        internal void handleEvent(EventType evt)
        {
            List<Transition<StateMachineType, EventType>> ts = null;
            Type type = evt.GetType();
            while (type != null && ts == null)
            {
                transitionsPerType.TryGetValue(type, out ts);
#if UNITY_WP_8_1
                type = type.GetTypeInfo().BaseType;
#else
                type = type.BaseType;
#endif

            }
            Assert.IsNotNull(ts, "This state shouldn't handle events of this type : State (" + GetType() + "), Event(" + evt.GetType() + ")");
            foreach (Transition<StateMachineType, EventType> t in ts)
            {
                State<StateMachineType, EventType> resultState;
                if (t.PassTransition(evt, out resultState))
                {
                    stateMachine.goTo(resultState);
                }
            }
        }
    }
}
