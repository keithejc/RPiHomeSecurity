using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace RPiHomeSecurity.Triggers
{
    public class InputStateTrigger : Trigger
    {
        public event TriggeredEventHandler TriggeredEventHandler;

        public string Name;
        public PinState TriggerState;
        private InputPin _input;

        public InputStateTrigger(String name, PinState triggerState)
        {
            this.Name = name;
            this.TriggerState = triggerState;
        }

        protected override void InitialiseTrigger()
        {
            Log.LogMessage("alarmController.GetInputPin(name); " + Name);
            _input = AlarmController.GetInputPin(Name);
            _input.InputChangedEventHandler += new InputChangedEventHandler(InputChangedEventHandler);
        }

        //the input has changed - if this mean's we have triggerd then pass this through the event handler
        protected virtual void InputChangedEventHandler(InputPin pin)
        {
            if (TriggeredEventHandler != null && IsTriggered())
            {
                Log.LogMessage(pin.Name + " triggered on " + TriggerState);

                TriggeredEventHandler.Invoke();
            }
        }

        protected void InvokeTriggeredEventHandler()
        {
            TriggeredEventHandler.Invoke();
        }

        public override bool IsTriggered()
        {
            return (_input.Value == TriggerState);
        }
    }
}