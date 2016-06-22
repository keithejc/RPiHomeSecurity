using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPiHomeSecurity
{
    internal class TurnOffOutputAction : Action
    {
        public String Output;

        public TurnOffOutputAction(String output)
        {
            this.Output = output;
        }

        public override void RunAction(Alarm alarmController)
        {
            var pin = alarmController.GetOutputPin(Output);
            if (pin != null)
            {
                Log.LogMessage("running Output Off Action. On " + pin.Name);
                pin.TurnOff();
            }
        }
    }
}