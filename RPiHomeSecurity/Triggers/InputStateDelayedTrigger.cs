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

        public int MsDelay;
        private Timer _delayTimer;
        public bool ResetDelayIfInputChanges;

        //trigger if the
        public InputStateDelayedTrigger(String name, PinState triggerState, int msDelay, bool resetDelayIfInputChanges)
            : base(name, triggerState)
        {
            this.MsDelay = msDelay;
            this.ResetDelayIfInputChanges = resetDelayIfInputChanges;

            _delayTimer = new Timer();
            if (msDelay > 0)
            {
                _delayTimer.Interval = msDelay;
            }
            _delayTimer.Elapsed += new ElapsedEventHandler(delayTimer_Elapsed);
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
            Log.LogMessage("InputChangedEventHandler " + pin.Name + " Value: " + pin.Value);
            //log.LogDebugMessage("TriggeredEventHandler " + (TriggeredEventHandler != null).ToString());
            if (IsTriggered())
            {
                Log.LogMessage(pin.Name + " triggered on " + TriggerState);

                //don't try and start the timer if it's already running
                if (MsDelay > 0 && !_delayTimer.Enabled)
                {
                    Log.LogMessage("Starting delay trigger on " + pin.Name + " for " + MsDelay + "ms");

                    _delayTimer.Start();
                }
                else
                {
                    InvokeTriggeredEventHandler();
                }
            }
            //if we have gone into a non-trigger state, stop any delay timer if wanted
            else if (ResetDelayIfInputChanges)
            {
                _delayTimer.Stop();
            }
        }
    }
}