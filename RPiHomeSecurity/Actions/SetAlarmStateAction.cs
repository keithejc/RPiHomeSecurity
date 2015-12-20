using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPiHomeSecurity
{
    internal class SetAlarmStateAction : Action
    {
        public bool setState;

        public SetAlarmStateAction(bool setState)
        {
            this.setState = setState;
        }

        public override void RunAction(Alarm AlarmController)
        {
            log.LogDebugMessage("running Set Alarm State action. " + setState);

            AlarmController.Alarmed = setState;
        }
    }
}