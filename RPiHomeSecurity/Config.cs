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
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RPiHomeSecurity
{
    public class Config
    {
        private static String ConfigFileName = "config.cfg";

        public String SmtpServer { get; set; }

        public String SmtpUserName { get; set; }

        public String SmtpPassword { get; set; }

        public String SmtpFromAddress { get; set; }

        public String EmailAddress { get; set; }

        public String EmailBody { get; set; }

        public String WebInterfacePort { get; set; }

        public Dictionary<String, int> OutputPins { get; set; }

        public Dictionary<String, int> InputPins { get; set; }

        public int AlarmSirenDuration { get; set; }
        public int WarningDuration  { get; set; }
        public int WarningToggleOn { get; set; }
        public int WarningToggleOff { get; set; }
        public int ChimeDuration { get; set; }

        public Config()
        {
            CreateDefaults();
        }

        private void CreateDefaults()
        {
            AlarmSirenDuration = 30000;
            WarningDuration = 7000;
            WarningToggleOn = 100;
            WarningToggleOff = 300;
            ChimeDuration = 100;

            InputPins = new Dictionary<string, int>();
            InputPins.Add("Door", 3);
            InputPins.Add("Handle", 11);

            OutputPins = new Dictionary<string, int>();
            OutputPins.Add("Siren", 13);
            OutputPins.Add("Light", 16);

            EmailAddress = "something@gmail.com";
            SmtpServer = "smtp.gmail.com";
            SmtpUserName = "something@gmail.com";
            SmtpPassword = "password";
            SmtpFromAddress = "something@gmail.com";
            EmailBody = "Any Useful stuff you want to email yourself. Eg a link to a camera etc.";
            WebInterfacePort = "33331";
        }

        //deserialize the JSON format config file to a Config object
        public static Config ReadConfigFile()
        {
            var fileLoc = AppDomain.CurrentDomain.BaseDirectory;
            var fileFullName = Path.Combine(fileLoc, ConfigFileName);

            Config config = null;

            try
            {
                ITraceWriter traceWriter = new MemoryTraceWriter();

                var json = System.IO.File.ReadAllText(fileFullName, Encoding.UTF8);
                config = JsonConvert.DeserializeObject<Config>(json);
            }
            catch (Exception e)
            {
                log.LogError("Failed to read config file. Should be in JSON format. And exist at: " + fileFullName + " " + e.ToString());
            }

            return config;
        }

        //look for the config file
        public static bool IsConfigFilePresent()
        {
            var fileLoc = AppDomain.CurrentDomain.BaseDirectory;
            var fileFullName = Path.Combine(fileLoc, ConfigFileName);
            return File.Exists(fileFullName);
        }

        //create a config file in JSON format with default values
        public static bool WriteConfigFile(Config config)
        {
            bool ret = false;

            var fileLoc = AppDomain.CurrentDomain.BaseDirectory;
            var fileFullName = Path.Combine(fileLoc, ConfigFileName);

            try
            {
                //create default values
                config.CreateDefaults();

                //write to JSON formatted config file
                String json = JsonConvert.SerializeObject(config, Formatting.Indented);
                System.IO.File.WriteAllText(fileFullName, json, Encoding.UTF8);
                ret = true;
            }
            catch (Exception e)
            {
                log.LogError("Failed to write config file " + fileFullName + " " + e.ToString());
            }

            return ret;
        }

        //create outputPin classes from GPIO connector pin numbers
        public Dictionary<string, IOutputPin> GetOutputPins()
        {
            var outputs = new Dictionary<String, IOutputPin>();

            foreach (var output in OutputPins)
            {
                outputs.Add(output.Key, new GpioOutputPin(output.Value, output.Key));
            }

            return outputs;
        }

        //create inputPin classes from GPIO pin numbers
        public Dictionary<string, IInputPin> GetInputPins()
        {
            var inputs = new Dictionary<String, IInputPin>();

            foreach (var input in InputPins)
            {
                inputs.Add(input.Key, new GpioInputPin(input.Value, input.Key));
            }

            return inputs;
        }

    }
}