using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPiHomeSecurity
{
    internal class TurnOffOutputAction : Action
    {
        public String output;

        public TurnOffOutputAction(String output)
        {
            this.output = output;
        }

        public override void RunAction(Alarm AlarmController)
        {
            var pin = AlarmController.GetOutputPin(output);
            if (pin != null)
            {
                log.LogDebugMessage("running Output Off Action. On " + pin.Name);
                pin.TurnOff();
            }
        }
    }
}