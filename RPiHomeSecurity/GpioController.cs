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
using Raspberry.IO.GeneralPurpose;

namespace RPiHomeSecurity
{
    public class GpioController : IoController
    {
        private GpioConnection connection;

        //setup the Raspberry.IO.GeneralPurpose driver with opur list of inputs and outputs
        public GpioController(Dictionary<String, InputPin> inputs, Dictionary<String, OutputPin> outputs)
            : base(inputs, outputs)
        {
            //outputs need to know the driver that is being used
            var driver = GpioConnectionSettings.DefaultDriver;
            foreach (var output in Outputs)
            {
                ((GpioOutputPin)(output.Value)).Driver = driver;
            }

            //connection needs to know the list of inputs we setup
            connection = new GpioConnection();
            foreach (var input in Inputs)
            {
                input.Value.inputChangedEventHandler += new InputChangedEventHandler(InputChanged);
                connection.Add(((GpioInputPin)input.Value).PinConfig);
            }
        }

        ~GpioController()
        {
            connection.Close();
        }

        //gpio pin header number to enum
        public static ConnectorPin IntToConnectorPin(int number)
        {
            switch (number)
            {
                case 3:
                    return ConnectorPin.P1Pin03;
                case 5:
                    return ConnectorPin.P1Pin05;
                case 7:
                    return ConnectorPin.P1Pin07;
                case 8:
                    return ConnectorPin.P1Pin08;
                case 10:
                    return ConnectorPin.P1Pin10;
                case 11:
                    return ConnectorPin.P1Pin11;
                case 12:
                    return ConnectorPin.P1Pin12;
                case 13:
                    return ConnectorPin.P1Pin13;
                case 15:
                    return ConnectorPin.P1Pin15;
                case 16:
                    return ConnectorPin.P1Pin16;
                case 18:
                    return ConnectorPin.P1Pin18;
                case 19:
                    return ConnectorPin.P1Pin19;
                case 21:
                    return ConnectorPin.P1Pin21;
                case 22:
                    return ConnectorPin.P1Pin22;
                case 23:
                    return ConnectorPin.P1Pin23;
                case 24:
                    return ConnectorPin.P1Pin24;
                case 26:
                    return ConnectorPin.P1Pin26;
                default:
                    throw new ArgumentOutOfRangeException("IntToConnectorPin: invalid gpio connector pin number");
            }
        }
    }
}