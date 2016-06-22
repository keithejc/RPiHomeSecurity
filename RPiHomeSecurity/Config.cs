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
using RPiHomeSecurity.Triggers;

namespace RPiHomeSecurity
{
    public class Config
    {
        public static String ArmActionListName = "Arm";
        public static String DisarmActionListName = "Disarm";
        public static String AlarmOnActionListName = "AlarmOn";
        public static String AlarmOffActionListName = "AlarmOff";
        public static String StartupActionListName = "Startup";
        public static String HandleWhileArmedActionListName = "HandleWhileArmed";
        public static String HandleWhileDisarmedActionListName = "HandleWhileDisarmed";

        public static String DoorOpenTriggerListName = "DoorOpenTrigger";
        public static String ArmInputTriggerListName = "ArmTrigger";
        public static String DisarmInputTriggerListName = "DisarmTrigger";
        public static String HandleWhileArmedInputTriggerListName = "HandleTrigger";
        public static String HandleWhileDisarmedInputTriggerListName = "HandleWhileDisarmedTrigger";

        public static String SirenOutputName = "Siren";
        public static String LightOutputName = "Light";

        public enum IoControllerTypes
        {
            RawGpio,
            PifaceDigital
        }

        private static String _configFileName = "config.cfg";

        public IoControllerTypes IoControllerType { get; set; }

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

        public int WarningDuration { get; set; }

        public int WarningToggleOn { get; set; }

        public int WarningToggleOff { get; set; }

        public int ChimeDuration { get; set; }

        public Dictionary<String, List<Action>> ActionLists { get; set; }

        public Dictionary<String, TriggerList> TriggerLists { get; set; }

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

            IoControllerType = IoControllerTypes.RawGpio;

            InputPins = new Dictionary<string, int>();
            InputPins.Add("Door", 3);
            InputPins.Add("Handle", 11);
            InputPins.Add("Arm", 8);
            InputPins.Add("DisArm", 10);
            InputPins.Add("MainsOk", 18);

            OutputPins = new Dictionary<string, int>();
            OutputPins.Add(SirenOutputName, 16);
            OutputPins.Add(LightOutputName, 13);

            EmailAddress = "something@gmail.com";
            SmtpServer = "smtp.gmail.com";
            SmtpUserName = "something@gmail.com";
            SmtpPassword = "password";
            SmtpFromAddress = "something@gmail.com";
            EmailBody = "Any Useful stuff you want to email yourself. Eg a link to a camera etc.";
            WebInterfacePort = "33331";

            SetupDefaultActionLists();
            SetupDefaultTriggerLists();
        }

        private void SetupDefaultTriggerLists()
        {
            TriggerLists = new Dictionary<string, TriggerList>();

            var doorTriggers = new List<Trigger>();
            doorTriggers.Add(new ArmStateTrigger(true));
            //if door opened and system is armed then go into alarm WarningDuration after door is opened, even if door is closed again
            var mainDoorTrigger = new InputStateDelayedTrigger("Door", PinState.High, WarningDuration, false);
            TriggerLists.Add(DoorOpenTriggerListName, new TriggerList(mainDoorTrigger, doorTriggers, AlarmOnActionListName));

            var mainArmTrigger = new InputStateTrigger("Arm", PinState.Low);
            TriggerLists.Add(ArmInputTriggerListName, new TriggerList(mainArmTrigger, new List<Trigger>(), ArmActionListName));

            var mainDisArmTrigger = new InputStateTrigger("DisArm", PinState.Low);
            TriggerLists.Add(DisarmInputTriggerListName, new TriggerList(mainDisArmTrigger, new List<Trigger>(), DisarmActionListName));

            var mainHandleWhileArmedTrigger = new InputStateTrigger("Handle", PinState.Low);
            var handleWhileArmedTriggers = new List<Trigger>();
            handleWhileArmedTriggers.Add(new ArmStateTrigger(true));
            TriggerLists.Add(HandleWhileArmedInputTriggerListName, new TriggerList(mainHandleWhileArmedTrigger, handleWhileArmedTriggers, HandleWhileArmedActionListName));

            var mainHandleWhileDisarmedTrigger = new InputStateTrigger("Handle", PinState.Low);
            var handleWhileDisarmedTriggers = new List<Trigger>();
            handleWhileDisarmedTriggers.Add(new ArmStateTrigger(false));
            TriggerLists.Add(HandleWhileDisarmedInputTriggerListName, new TriggerList(mainHandleWhileDisarmedTrigger, handleWhileDisarmedTriggers, HandleWhileDisarmedActionListName));
        }

        private void SetupDefaultActionLists()
        {
            ActionLists = new Dictionary<string, List<Action>>();
            var armActions = new List<Action>();
            armActions.Add(new SetArmStateAction(true));
            armActions.Add(new TurnOnOutputAction(LightOutputName, 1000));
            armActions.Add(new ToggleOutputAction(SirenOutputName, ChimeDuration, 200, 10));
            ActionLists.Add(ArmActionListName, armActions);

            var disarmActions = new List<Action>();
            disarmActions.Add(new SetArmStateAction(false));
            disarmActions.Add(new SetAlarmStateAction(false));
            disarmActions.Add(new TurnOffOutputAction(SirenOutputName));
            disarmActions.Add(new TurnOnOutputAction(LightOutputName, 1000));
            disarmActions.Add(new ToggleOutputAction(SirenOutputName, ChimeDuration, 200, 3));
            ActionLists.Add(DisarmActionListName, disarmActions);

            var alarmOnActions = new List<Action>();
            alarmOnActions.Add(new SetAlarmStateAction(true));
            alarmOnActions.Add(new TurnOnOutputAction(SirenOutputName, AlarmSirenDuration));
            alarmOnActions.Add(new EmailAction("Alarm", ""));
            ActionLists.Add(AlarmOnActionListName, alarmOnActions);

            var alarmOffActions = new List<Action>();
            alarmOffActions.Add(new SetAlarmStateAction(false));
            alarmOffActions.Add(new TurnOffOutputAction(SirenOutputName));
            ActionLists.Add(AlarmOffActionListName, alarmOffActions);

            var startupActions = new List<Action>();
            startupActions.Add(new EmailAction("Startup", ""));
            startupActions.Add(new SetArmStateAction(true));
            startupActions.Add(new TurnOnOutputAction(LightOutputName, 1000));
            startupActions.Add(new ToggleOutputAction(SirenOutputName, ChimeDuration, 200, 10));
            ActionLists.Add(StartupActionListName, startupActions);

            var handleWhileArmedActions = new List<Action>();
            handleWhileArmedActions.Add(new EmailAction("Handle", ""));
            handleWhileArmedActions.Add(new TurnOnOutputAction(LightOutputName, 10000));
            handleWhileArmedActions.Add(new ToggleOutputAction(SirenOutputName, ChimeDuration, 200, 10));
            ActionLists.Add(HandleWhileArmedActionListName, handleWhileArmedActions);

            var handleWhileDisarmedActions = new List<Action>();
            handleWhileDisarmedActions.Add(new TurnOnOutputAction(LightOutputName, 10000));
            handleWhileDisarmedActions.Add(new ToggleOutputAction(SirenOutputName, ChimeDuration, 200, 2));
            ActionLists.Add(HandleWhileDisarmedActionListName, handleWhileDisarmedActions);
        }

        //deserialize the JSON format config file to a Config object
        public static Config ReadConfigFile()
        {
            var fileLoc = AppDomain.CurrentDomain.BaseDirectory;
            var fileFullName = Path.Combine(fileLoc, _configFileName);

            Config config = null;

            try
            {
                ITraceWriter traceWriter = new MemoryTraceWriter();

                var json = System.IO.File.ReadAllText(fileFullName, Encoding.UTF8);
                config = JsonConvert.DeserializeObject<Config>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
            }
            catch (Exception e)
            {
                Log.LogError("Failed to read config file. Should be in JSON format. And exist at: " + fileFullName + " " + e.ToString());
            }

            return config;
        }

        //look for the config file
        public static bool IsConfigFilePresent()
        {
            var fileLoc = AppDomain.CurrentDomain.BaseDirectory;
            var fileFullName = Path.Combine(fileLoc, _configFileName);
            return File.Exists(fileFullName);
        }

        //create a config file in JSON format with default values
        public static bool WriteConfigFile(Config config)
        {
            var ret = false;

            var fileLoc = AppDomain.CurrentDomain.BaseDirectory;
            var fileFullName = Path.Combine(fileLoc, _configFileName);

            try
            {
                //create default values
                config.CreateDefaults();

                //write to JSON formatted config file
                var json = JsonConvert.SerializeObject(config, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                System.IO.File.WriteAllText(fileFullName, json, Encoding.UTF8);
                ret = true;
            }
            catch (Exception e)
            {
                Log.LogError("Failed to write config file " + fileFullName + " " + e.ToString());
            }

            return ret;
        }
    }
}