using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPiHomeSecurity
{
    public class TurnOnOutputAction : Action
    {
        public String Output;
        public int Duration;

        public TurnOnOutputAction(String output, int msDuration)
        {
            this.Output = output;
            this.Duration = msDuration;
        }

        public override void RunAction(Alarm alarmController)
        {
            var pin = alarmController.GetOutputPin(Output);
            if (pin != null)
            {
                Log.LogMessage("running Output On Action. On " + pin.Name);
                pin.TurnOn(Duration);
            }
        }
    }
}