using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPiHomeSecurity
{
    internal class ToggleOutputAction : Action
    {
        public String Output;
        public int MsOffTime;
        public int NumToggles;
        public int MsOnTime;

        public ToggleOutputAction(String output, int msOnTime, int msOffTime, int numToggles)
        {
            this.Output = output;
            this.MsOnTime = msOnTime;
            this.MsOffTime = msOffTime;
            this.NumToggles = numToggles;
        }

        public override void RunAction(Alarm alarmController)
        {
            var pin = alarmController.GetOutputPin(Output);
            if (pin != null)
            {
                Log.LogMessage("running Toggle Output Action. On " + pin.Name + ". msOnTime" + MsOnTime + " msOffTime:" + MsOffTime + " numToggles:" + NumToggles);

                pin.Toggle(MsOnTime, MsOffTime, NumToggles);
            }
        }
    }
}