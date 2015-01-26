using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPiHomeSecurity
{
    public class TurnOnOutputAction : Action
    {
        private String output;
        private int duration;

        public TurnOnOutputAction(String output, int msDuration)
        {
            this.output = output;
            this.duration = msDuration;
        }

        public override void RunAction(Alarm AlarmController)
        {
            var pin = AlarmController.GetOutputPin(output);
            if (pin != null)
            {
                log.LogDebugMessage("running Output On Action. On " + pin.Name);
                pin.TurnOn(duration);
            }
        }
    }
}