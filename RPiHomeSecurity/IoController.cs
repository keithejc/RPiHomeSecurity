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
using System.Collections.Generic;

namespace RPiHomeSecurity
{
    public abstract class IoController
    {
        public event InputChangedEventHandler inputChangedEventHandler;

        //outputs
        public Dictionary<String, OutputPin> Outputs { get; set; }

        //inputs
        public Dictionary<String, InputPin> Inputs { get; set; }

        //setup the GPIO driver with our list of inputs and outputs
        public IoController(Dictionary<String, InputPin> inputs, Dictionary<String, OutputPin> outputs)
        {
            Inputs = inputs;
            Outputs = outputs;
        }

        //one of the inputs changed - pass event on
        public void InputChanged(InputPin pin)
        {
            if (inputChangedEventHandler != null)
            {
                inputChangedEventHandler.Invoke(pin);
            }
        }

        public void Toggle(String name, int msOnTime, int msOffTime, int numToggles)
        {
            OutputPin outPin = Outputs[name];
            if (outPin != null)
            {
                outPin.Toggle(msOnTime, msOffTime, numToggles);
            }
        }

        public void TurnOnOutput(String name, int msDuration)
        {
            OutputPin outPin = Outputs[name];
            if (outPin != null)
            {
                outPin.TurnOn(msDuration);
            }
        }

        public void TurnOffOutput(String name)
        {
            OutputPin outPin = Outputs[name];
            if (outPin != null)
            {
                outPin.TurnOff();
            }
        }
    }
}