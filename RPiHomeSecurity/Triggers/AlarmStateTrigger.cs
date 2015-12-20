using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPiHomeSecurity.Triggers
{
    public class AlarmStateTrigger : Trigger
    {
        public bool alarmTriggerState;

        public AlarmStateTrigger(bool alarmTriggerState)
        {
            this.alarmTriggerState = alarmTriggerState;
        }

        protected override void InitialiseTrigger()
        {
        }

        public override bool IsTriggered()
        {
            return alarmController.Armed == alarmTriggerState;
        }
    }
}