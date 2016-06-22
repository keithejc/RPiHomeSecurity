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
    public class IoControllerFactory
    {
        //depending on the type of io we're using, create an io controller
        public static IoController CreateIoController(Config config)
        {
            IoController ioController = null;

            switch (config.IoControllerType)
            {
                case Config.IoControllerTypes.RawGpio:
                    Log.LogMessage("Using Raw GPIO");
                    ioController = new GpioController(IoControllerFactory.GetGpioInputPins(config), IoControllerFactory.GetGpioOutputPins(config));
                    break;

                case Config.IoControllerTypes.PifaceDigital:
                    Log.LogMessage("Using PifaceDigital");
                    ioController = new PiFaceIoController(IoControllerFactory.GetPiFaceInputPins(config), IoControllerFactory.GetPiFaceOutputPins(config));
                    break;

                default:
                    throw new ArgumentException("Config does not contain a valid IoController");
            }

            return ioController;
        }

        //create GpioOutputPin classes from GPIO connector pin numbers
        public static Dictionary<string, OutputPin> GetGpioOutputPins(Config config)
        {
            var outputs = new Dictionary<String, OutputPin>();

            foreach (var output in config.OutputPins)
            {
                outputs.Add(output.Key, new GpioOutputPin(output.Value, output.Key));
            }

            return outputs;
        }

        //create GpioInputPin classes from GPIO pin numbers
        public static Dictionary<string, InputPin> GetGpioInputPins(Config config)
        {
            var inputs = new Dictionary<String, InputPin>();

            foreach (var input in config.InputPins)
            {
                inputs.Add(input.Key, new GpioInputPin(input.Value, input.Key));
            }

            return inputs;
        }

        //create PiFaceOutputPin classes from output numbers
        public static Dictionary<string, OutputPin> GetPiFaceOutputPins(Config config)
        {
            var outputs = new Dictionary<String, OutputPin>();

            foreach (var output in config.OutputPins)
            {
                outputs.Add(output.Key, new PiFaceOutputPin(output.Value, output.Key));
            }

            return outputs;
        }

        //create PiFaceInputPin classes from input numbers
        public static Dictionary<string, InputPin> GetPiFaceInputPins(Config config)
        {
            var inputs = new Dictionary<String, InputPin>();

            foreach (var input in config.InputPins)
            {
                inputs.Add(input.Key, new PiFaceInputPin(input.Value, input.Key));
            }

            return inputs;
        }
    }
}