using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPiHomeSecurity.Triggers
{
    public class AlarmStateTrigger : Trigger
    {
        public bool AlarmTriggerState;

        public AlarmStateTrigger(bool alarmTriggerState)
        {
            this.AlarmTriggerState = alarmTriggerState;
        }

        protected override void InitialiseTrigger()
        {
        }

        public override bool IsTriggered()
        {
            return AlarmController.Armed == AlarmTriggerState;
        }
    }
}