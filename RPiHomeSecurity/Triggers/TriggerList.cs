using System;
using System.Collections.Generic;

namespace RPiHomeSecurity.Triggers
{
    public class TriggerList
    {
        public List<Trigger> triggers;

        public string ActionList { get; set; }

        public InputStateTrigger MainTrigger { get; set; }

        private Alarm alarmController;

        public TriggerList(InputStateTrigger mainTrigger, List<Trigger> triggers, String actionList)
        {
            MainTrigger = mainTrigger;
            this.triggers = triggers;
            ActionList = actionList;
        }

        //initialise each trigger and fix up event handlers
        public void Initialise(Alarm alarmController)
        {
            this.alarmController = alarmController;
            //fix up the main trigger's event handler
            MainTrigger.Initialise(alarmController);
            MainTrigger.TriggeredEventHandler += new TriggeredEventHandler(MainTrigger_TriggeredEventHandler);

            foreach (var trigger in triggers)
            {
                trigger.Initialise(alarmController);
            }
        }

        //main trigger has fired, check to see if all other triggers are triggered too
        private void MainTrigger_TriggeredEventHandler()
        {
            if (IsTriggered())
            {
                alarmController.RunActionList(ActionList);
            }
        }

        //and together all the other triggers
        public bool IsTriggered()
        {
            bool triggered = true;
            foreach (var trigger in triggers)
            {
                if (!trigger.IsTriggered())
                {
                    triggered = false;
                }
            }
            return triggered;
        }
    }
}