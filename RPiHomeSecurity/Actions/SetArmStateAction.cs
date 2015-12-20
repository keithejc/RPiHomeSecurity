using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPiHomeSecurity
{
    public class SetArmStateAction : Action
    {
        public bool setState;

        public SetArmStateAction(bool setState)
        {
            this.setState = setState;
        }

        public override void RunAction(Alarm AlarmController)
        {
            log.LogDebugMessage("running Set Arm State action. " + setState);
            AlarmController.Armed = setState;
        }
    }
}