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

        protected PinState _pinState;
        public PinState Value { get { return _pinState; } }
        public int PinNumber { get; set; }

        protected System.Timers.Timer OffTimer;
        protected System.Timers.Timer ToggleTimer;
        protected int NumToggles;
        protected int MsToggleOnTime;
        protected int MsToggleOffTime;
        protected bool IsToggleOutputOn;

        public OutputPin(int pinNumber, String name)
        {
            Name = name;
            PinNumber = pinNumber;

            OffTimer = new System.Timers.Timer();
            OffTimer.Elapsed += OffTimerElapsed;
            OffTimer.AutoReset = false;

            ToggleTimer = new System.Timers.Timer();
            ToggleTimer.Elapsed += ToggleTimerElapsed;
            ToggleTimer.AutoReset = false;
            NumToggles = 0;
        }

        //toggles the output for a number of toggles
        //typically for alarm siren warning
        public void Toggle(int msOnTime, int msOffTime, int numToggles)
        {
            this.NumToggles = numToggles;
            IsToggleOutputOn = true;

            MsToggleOnTime = msOnTime;
            MsToggleOffTime = msOffTime;

            ToggleTimer.Interval = msOnTime;
            ToggleTimer.Start();
        }

        //turn on the output for a length of time
        //or 0 == indefinitely
        public void TurnOn(int msDuration)
        {
            //if output is toggling - stop that
            ToggleTimer.Stop();

            if (msDuration > 0)
            {
                OffTimer.Interval = msDuration;
                OffTimer.Start();
            }

            OutputOn();
        }

        //turn off the output
        public void TurnOff()
        {
            OffTimer.Stop();
            ToggleTimer.Stop();

            OutputOff();
        }

        //the timer's elapsed - turn on / off the output or finish
        private void ToggleTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (IsToggleOutputOn)
            {
                TurnOff();
                NumToggles--;
            }

            if (NumToggles >= 0)
            {
                if (IsToggleOutputOn)
                {
                    ToggleTimer.Interval = MsToggleOffTime;
                    ToggleTimer.Start();
                }
                else
                {
                    OutputOn();
                    ToggleTimer.Interval = MsToggleOnTime;
                    ToggleTimer.Start();
                }

                IsToggleOutputOn = !IsToggleOutputOn;
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