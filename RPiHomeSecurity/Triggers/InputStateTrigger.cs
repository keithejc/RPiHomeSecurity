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

        public string name;
        public PinState triggerState;
        private InputPin input;

        public InputStateTrigger(String name, PinState triggerState)
        {
            this.name = name;
            this.triggerState = triggerState;
        }

        protected override void InitialiseTrigger()
        {
            log.LogDebugMessage("alarmController.GetInputPin(name); " + name);
            input = alarmController.GetInputPin(name);
            input.inputChangedEventHandler += new InputChangedEventHandler(InputChangedEventHandler);
        }

        //the input has changed - if this mean's we have triggerd then pass this through the event handler
        protected virtual void InputChangedEventHandler(InputPin pin)
        {
            if (TriggeredEventHandler != null && IsTriggered())
            {
                log.LogDebugMessage(pin.Name + " triggered on " + triggerState);

                TriggeredEventHandler.Invoke();
            }
        }

        protected void InvokeTriggeredEventHandler()
        {
            TriggeredEventHandler.Invoke();
        }

        public override bool IsTriggered()
        {
            return (input.Value == triggerState);
        }
    }
}