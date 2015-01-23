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
using Raspberry.IO.GeneralPurpose;

namespace RPiHomeSecurity
{
    internal class GpioOutputPin : OutputPin
    {
        private IGpioConnectionDriver driver;

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

        public GpioOutputPin(int pinNumber, String name)
            : base(pinNumber, name)
        {
            connectorPin = GpioController.IntToConnectorPin(pinNumber);
            processorPin = connectorPin.ToProcessor();

            log.LogDebugMessage("Output " + name + " on " + connectorPin);
        }

        ~GpioOutputPin()
        {
            TurnOff();
            driver.Release(processorPin);
        }

        //turn on the output using Raspberry.IO.GeneralPurpose library
        protected override void OutputOn()
        {
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

        //turn off the output using Raspberry.IO.GeneralPurpose library
        protected override void OutputOff()
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