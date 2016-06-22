using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPiHomeSecurity
{
    internal class SetAlarmStateAction : Action
    {
        public bool SetState;

        public SetAlarmStateAction(bool setState)
        {
            this.SetState = setState;
        }

        public override void RunAction(Alarm alarmController)
        {
            Log.LogMessage("running Set Alarm State action. " + SetState);

            alarmController.Alarmed = SetState;
        }
    }
}