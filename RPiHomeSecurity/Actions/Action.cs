using System.Collections;

namespace RPiHomeSecurity
{
    public abstract class Action
    {
        public abstract void RunAction(Alarm AlarmController);
    }
}