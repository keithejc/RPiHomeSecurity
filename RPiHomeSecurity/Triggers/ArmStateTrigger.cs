using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPiHomeSecurity.Triggers
{
    public class ArmStateTrigger : Trigger
    {
        public bool ArmTriggerState;

        public ArmStateTrigger(bool armTriggerState)
        {
            this.ArmTriggerState = armTriggerState;
        }

        protected override void InitialiseTrigger()
        {
        }

        public override bool IsTriggered()
        {
            return AlarmController.Armed == ArmTriggerState;
        }
    }
}