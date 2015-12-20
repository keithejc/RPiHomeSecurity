using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace RPiHomeSecurity.Triggers
{
    public class InputStateDelayedTrigger : InputStateTrigger
    {
        //new public event TriggeredEventHandler TriggeredEventHandler;

        public int msDelay;
        private Timer delayTimer;
        public bool resetDelayIfInputChanges;

        //trigger if the
        public InputStateDelayedTrigger(String name, PinState triggerState, int msDelay, bool resetDelayIfInputChanges)
            : base(name, triggerState)
        {
            this.msDelay = msDelay;
            this.resetDelayIfInputChanges = resetDelayIfInputChanges;

            delayTimer = new Timer();
            if (msDelay > 0)
            {
                delayTimer.Interval = msDelay;
            }
            delayTimer.Elapsed += new ElapsedEventHandler(delayTimer_Elapsed);
        }

        private void delayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsTriggered())
            {
                InvokeTriggeredEventHandler();
            }
        }

        //the input has changed - if this mean's we have triggerd then pass this through the event handler
        //if there is a delay setup then the trigger won't happen till the time is up
        protected override void InputChangedEventHandler(InputPin pin)
        {
            log.LogDebugMessage("InputChangedEventHandler " + pin.Name + " Value: " + pin.Value);
            //log.LogDebugMessage("TriggeredEventHandler " + (TriggeredEventHandler != null).ToString());
            if (IsTriggered())
            {
                log.LogDebugMessage(pin.Name + " triggered on " + triggerState);

                //don't try and start the timer if it's already running
                if (msDelay > 0 && !delayTimer.Enabled)
                {
                    log.LogDebugMessage("Starting delay trigger on " + pin.Name + " for " + msDelay + "ms");

                    delayTimer.Start();
                }
                else
                {
                    InvokeTriggeredEventHandler();
                }
            }
            //if we have gone into a non-trigger state, stop any delay timer if wanted
            else if (resetDelayIfInputChanges)
            {
                delayTimer.Stop();
            }
        }
    }
}