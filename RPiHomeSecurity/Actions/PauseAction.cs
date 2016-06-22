using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RPiHomeSecurity
{
    public class PauseAction : Action
    {
        public int MsDelay;

        public PauseAction(int msDelay)
        {
            this.MsDelay = msDelay;
        }

        public override void RunAction(Alarm alarmController)
        {
            Log.LogMessage("running pause action. Delay: " + MsDelay);

            Thread.Sleep(MsDelay);
        }
    }
}