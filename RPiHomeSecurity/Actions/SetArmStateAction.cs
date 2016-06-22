using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPiHomeSecurity
{
    public class SetArmStateAction : Action
    {
        public bool SetState;

        public SetArmStateAction(bool setState)
        {
            this.SetState = setState;
        }

        public override void RunAction(Alarm alarmController)
        {
            Log.LogMessage("running Set Arm State action. " + SetState);
            alarmController.Armed = SetState;
        }
    }
}