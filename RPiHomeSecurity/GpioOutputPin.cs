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
        private IGpioConnectionDriver _driver;

        public IGpioConnectionDriver Driver
        {
            set
            {
                _driver = value;
                _driver.Allocate(_processorPin, PinDirection.Output);
            }
        }

        private ConnectorPin _connectorPin;
        private ProcessorPin _processorPin;

        public GpioOutputPin(int pinNumber, String name)
            : base(pinNumber, name)
        {
            _connectorPin = GpioController.IntToConnectorPin(pinNumber);
            _processorPin = _connectorPin.ToProcessor();

            Log.LogMessage("Output " + name + " on " + _connectorPin);
        }

        ~GpioOutputPin()
        {
            TurnOff();
            _driver.Release(_processorPin);
        }

        //turn on the output using Raspberry.IO.GeneralPurpose library
        protected override void OutputOn()
        {
            //don't allow GPIO pins to be accessed at the same time
            lock (PinFileLock.Instance)
            {
                try
                {
                    Log.LogMessage("Turn on " + _connectorPin.ToString());
                    _driver.Write(_processorPin, true);
                    _pinState = PinState.High;
                }
                catch (Exception e)
                {
                    Log.LogError("Failed to turn On " + _connectorPin.ToString() + " Error: " + e.ToString());
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
                    Log.LogMessage("Output off. " + _connectorPin);
                    _driver.Write(_processorPin, false);
                    _pinState = PinState.Low;
                }
                catch (Exception e)
                {
                    Log.LogError("Failed to turn Off " + _connectorPin.ToString() + " Error: " + e.ToString());
                }
            }
        }
    }
}