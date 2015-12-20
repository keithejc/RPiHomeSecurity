using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RPiHomeSecurity
{
    public class PauseAction : Action
    {
        public int msDelay;

        public PauseAction(int msDelay)
        {
            this.msDelay = msDelay;
        }

        public override void RunAction(Alarm AlarmController)
        {
            log.LogDebugMessage("running pause action. Delay: " + msDelay);

            Thread.Sleep(msDelay);
        }
    }
}