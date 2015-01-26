using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPiHomeSecurity.Triggers
{
    public class ArmStateTrigger : Trigger
    {
        private bool armTriggerState;

        public ArmStateTrigger(bool armTriggerState)
        {
            this.armTriggerState = armTriggerState;
        }

        protected override void InitialiseTrigger()
        {
        }

        public override bool IsTriggered()
        {
            return alarmController.Armed == armTriggerState;
        }
    }
}