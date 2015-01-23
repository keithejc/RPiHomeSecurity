/*******************************************************************************
 * Copyright 2015 Keith Cassidy
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 ******************************************************************************/

using System;
using System.Timers;

namespace RPiHomeSecurity
{
    public abstract class OutputPin
    {
        public String Name { get; set; }

        public int PinNumber { get; set; }

        protected System.Timers.Timer offTimer;
        protected System.Timers.Timer toggleTimer;
        protected int numToggles;
        protected int msToggleOnTime;
        protected int msToggleOffTime;
        protected bool isToggleOutputOn;

        public OutputPin(int pinNumber, String name)
        {
            Name = name;
            PinNumber = pinNumber;

            offTimer = new System.Timers.Timer();
            offTimer.Elapsed += OffTimerElapsed;
            offTimer.AutoReset = false;

            toggleTimer = new System.Timers.Timer();
            toggleTimer.Elapsed += ToggleTimerElapsed;
            toggleTimer.AutoReset = false;
            numToggles = 0;
        }

        //toggles the output for a number of toggles
        //typically for alarm siren warning
        public void Toggle(int msOnTime, int msOffTime, int numToggles)
        {
            this.numToggles = numToggles;
            isToggleOutputOn = true;

            msToggleOnTime = msOnTime;
            msToggleOffTime = msOffTime;

            toggleTimer.Interval = msOnTime;
            toggleTimer.Start();
        }

        //turn on the output for a length of time
        //or 0 == indefinitely
        public void TurnOn(int msDuration)
        {
            //if output is toggling - stop that
            toggleTimer.Stop();

            if (msDuration > 0)
            {
                offTimer.Interval = msDuration;
                offTimer.Start();
            }

            OutputOn();
        }

        //turn off the output
        public void TurnOff()
        {
            offTimer.Stop();
            toggleTimer.Stop();

            OutputOff();
        }

        //the timer's elapsed - turn on / off the output or finish
        private void ToggleTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (isToggleOutputOn)
            {
                TurnOff();
                numToggles--;
            }

            if (numToggles >= 0)
            {
                if (isToggleOutputOn)
                {
                    toggleTimer.Interval = msToggleOffTime;
                    toggleTimer.Start();
                }
                else
                {
                    OutputOn();
                    toggleTimer.Interval = msToggleOnTime;
                    toggleTimer.Start();
                }

                isToggleOutputOn = !isToggleOutputOn;
            }
        }

        //the timer's elapsed - turn off the output
        protected void OffTimerElapsed(object sender, ElapsedEventArgs e)
        {
            TurnOff();
        }

        protected abstract void OutputOn();

        protected abstract void OutputOff();
    }
}