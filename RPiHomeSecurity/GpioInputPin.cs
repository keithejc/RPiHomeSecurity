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
    internal class GpioInputPin : InputPin
    {
        private PinConfiguration _pin;

        public PinConfiguration PinConfig { get { return _pin; } }

        //setup the event handler for changes - using Raspberry.IO.GeneralPurpose library
        public GpioInputPin(int pinNumber, String name)
            : base(pinNumber, name)
        {
            var gpioPin = GpioController.IntToConnectorPin(pinNumber);
            _pin = gpioPin.Input()
                .Name(gpioPin.ToString())
                .OnStatusChanged(b =>
                {
                    InputChanged(b);
                });

            Log.LogMessage("GpioInputPin CTOR() " + name + " on " + gpioPin);
        }

        ~GpioInputPin()
        {
            Log.LogMessage("GpioInputPin DTOR() " + _pin.Name);
        }
    }
}