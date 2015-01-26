using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPiHomeSecurity
{
    public delegate void TriggeredEventHandler();

    public abstract class Trigger
    {
        protected Alarm alarmController;

        public void Initialise(Alarm alarmController)
        {
            this.alarmController = alarmController;

            InitialiseTrigger();
        }

        //initialise the derived class
        protected abstract void InitialiseTrigger();

        //has the trigger condition been met?
        //eg is the input in the slected state
        public abstract bool IsTriggered();
    }
}