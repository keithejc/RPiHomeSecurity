using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPiHomeSecurity
{
    internal class ToggleOutputAction : Action
    {
        public String output;
        public int msOffTime;
        public int numToggles;
        public int msOnTime;

        public ToggleOutputAction(String output, int msOnTime, int msOffTime, int numToggles)
        {
            this.output = output;
            this.msOnTime = msOnTime;
            this.msOffTime = msOffTime;
            this.numToggles = numToggles;
        }

        public override void RunAction(Alarm AlarmController)
        {
            var pin = AlarmController.GetOutputPin(output);
            if (pin != null)
            {
                log.LogDebugMessage("running Toggle Output Action. On " + pin.Name + ". msOnTime" + msOnTime + " msOffTime:" + msOffTime + " numToggles:" + numToggles);

                pin.Toggle(msOnTime, msOffTime, numToggles);
            }
        }
    }
}