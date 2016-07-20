using UnityEngine;
using UnityEngine.Assertions;

namespace GenericStateMachine
{
    /**
     * This class makes it easy to create state machine and is composed of States will fire specific Transitions depending on the Events that are received.
     * If you convert a drawn state machine to this data structure, each circle will be a new State class and each arrow a new Transition.
     * 
     * The machine is associated with a class of Events that are passed to it through its handleEvent method.
     * Each time a new Event is received, it will be passed to the current State, depending on the Event's class, the corresponding Transition will be fired.
     **/
    public abstract class StateMachine<StateMachineType,EventType> where StateMachineType : StateMachine<StateMachineType,EventType>
    {

        internal State<StateMachineType, EventType> current = null;
        internal State<StateMachineType, EventType> first = null;

        public StateMachine()
        {
            Init();
        }

        protected virtual void Init()
        {
            InitFirstState();
        }

        protected void InitFirstState()
        {
            first = DefineFirst();
            Assert.IsNotNull(first);
            current = first;
            first.enter();
        }

        internal abstract State<StateMachineType, EventType> DefineFirst();

        public void handleEvent(EventType evt)
        {
            current.handleEvent(evt);
        }

        internal virtual void goTo(State<StateMachineType, EventType> s)
        {
            current.leave();
            current = s;
            current.enter();
        }
    }
}