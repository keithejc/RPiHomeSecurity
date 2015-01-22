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
using Raspberry.IO.GeneralPurpose;

namespace RPiHomeSecurity
{
    internal class GpioOutputPin : IOutputPin
    {
        private IGpioConnectionDriver driver;
        public String Name { get; set; }

        public IGpioConnectionDriver Driver
        {
            set
            {
                driver = value;
                driver.Allocate(processorPin, PinDirection.Output);
            }
        }

        private ConnectorPin connectorPin;
        private ProcessorPin processorPin;

        private System.Timers.Timer offTimer;
        private System.Timers.Timer toggleTimer;
        private int numToggles;
        private int msToggleOnTime;
        private int msToggleOffTime;
        private bool isToggleOutputOn;

        public GpioOutputPin(int pinNumber, String name)
        {
            Name = name;
            connectorPin = GpioController.IntToConnectorPin(pinNumber);
            processorPin = connectorPin.ToProcessor();

            offTimer = new System.Timers.Timer();
            offTimer.Elapsed += OffTimerElapsed;
            offTimer.AutoReset = false;

            toggleTimer = new System.Timers.Timer();
            toggleTimer.Elapsed += ToggleTimerElapsed;
            toggleTimer.AutoReset = false;
            numToggles = 0;
            
        }

        ~GpioOutputPin()
        {
            TurnOff();
            driver.Release(processorPin);
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
            if (msDuration > 0)
            {
                offTimer.Interval = msDuration;
                offTimer.Start();
            }

            OutputOn();
        }

        private void OutputOn()
        {

            //if output is toggling - stop that
            toggleTimer.Stop();

            //don't allow GPIO pins to be accessed at the same time
            lock (PinFileLock.Instance)
            {
                try
                {
                    log.LogDebugMessage("Turn on " + connectorPin.ToString());
                    driver.Write(processorPin, true);
                }
                catch (Exception e)
                {
                    log.LogError("Failed to turn On " + connectorPin.ToString() + " Error: " + e.ToString());
                }
            }
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
        private void OffTimerElapsed(object sender, ElapsedEventArgs e)
        {
            TurnOff();
        }

        //turn off the output
        public void TurnOff()
        {
            offTimer.Stop();
            toggleTimer.Stop();

            OutputOff();
        }

        private void OutputOff()
        {
            //don't allow GPIO pins to be accessed at the same time
            lock (PinFileLock.Instance)
            {
                try
                {
                    log.LogDebugMessage("Output off. " + connectorPin);
                    driver.Write(processorPin, false);
                }
                catch (Exception e)
                {
                    log.LogError("Failed to turn Off " + connectorPin.ToString() + " Error: " + e.ToString());
                }
            }

        }
    }
}