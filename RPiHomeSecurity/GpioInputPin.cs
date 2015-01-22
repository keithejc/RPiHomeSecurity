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
    internal class GpioInputPin : IInputPin
    {
        private PinConfiguration pin;

        public PinConfiguration PinConfig { get { return pin; } }

        public PinState Value { get; set; }
        public String Name { get; set; }

        //evant handler to call if the input changes
        public event InputChangedEventHandler inputChangedEventHandler;

        public GpioInputPin(int pinNumber, String name)
        {
            Name = name;
            ConnectorPin gpioPin = GpioController.IntToConnectorPin(pinNumber);
            pin = gpioPin.Input()
                .Name(gpioPin.ToString())
                .OnStatusChanged(b =>
                {
                    OnInputChanged(b);
                });
        }

        private void OnInputChanged(bool newValue)
        {
            log.LogDebugMessage("Input " + pin.Name + " changed to: " + (newValue ? "High" : "Low"));
            Value = newValue ? PinState.High : PinState.Low;

            if (inputChangedEventHandler != null)
            {
                inputChangedEventHandler.Invoke(this);
            }
        }
   }
}