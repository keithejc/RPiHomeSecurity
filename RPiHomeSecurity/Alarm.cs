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
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Web;
using System.Timers;
using RPiHomeSecurity.Utils;
using RPiHomeSecurity.wamp;

namespace RPiHomeSecurity
{

    public struct StatusPinState
    {
        public String Name;
        public PinState State;
    }

    public struct RPiHomeSecurityStatus
    {
        public bool InAlarm;
        public bool Armed;
        public List<StatusPinState> Inputs;
        public List<StatusPinState> Outputs;
    }

    public class Alarm
    {
        public bool Armed { get; set; }

        public bool Alarmed { get; set; }

        public IoController IoBoard { get; set; }

        public Config Config { get; set; }

        private System.Timers.Timer _loggerStatusTimer;
        private const double StatusTimerInterval = 1000;

        //setup the alarm system
        public Alarm(Config configuration)
        {
            Alarmed = false;

            //get configuration
            Config = configuration;

            //get the io controller we're going to use
            IoBoard = IoControllerFactory.CreateIoController(Config);

            //make sure the triggers are started
            InitialiseTriggers();

            Log.LogMessage("Rpi Home Security Ready...");

            _loggerStatusTimer = new System.Timers.Timer(StatusTimerInterval);
            _loggerStatusTimer.AutoReset = true;
            _loggerStatusTimer.Elapsed += OnSystemStatusTimerEvent;
            _loggerStatusTimer.Start();


            //run the startup actions
            RunActionList(Config.StartupActionListName);
        }


        public InputPin GetInputPin(string name)
        {
            return IoBoard.Inputs[name];
        }

        //get the outputpin from the name
        public OutputPin GetOutputPin(String name)
        {
            return IoBoard.Outputs[name];
        }

        //link all triggers to this alarmcontroller and make them set themselves up to start triggering
        private void InitialiseTriggers()
        {
            foreach (var triggerList in Config.TriggerLists)
            {
                triggerList.Value.Initialise(this);
            }
        }

        private void SendEmail(String subject, String body)
        {
            new EmailAction(subject, body).RunAction(this);
        }

        //run a series of actions
        public void RunActionList(String actionList)
        {
            List<Action> actions;
            if( Config.ActionLists.TryGetValue(actionList, out actions) )
            {
                Log.LogMessage("Running action list: " + actionList);

                foreach (var action in actions)
                {
                    action.RunAction(this);
                }
            }
            else
            {
                Log.LogMessage(String.Format("Unknown action list ({0}). Is it spelled correctly? Correct case ?: " , actionList));
            }

        }

        public delegate void PublishRpiSystemStatus(RPiHomeSecurityStatus status);
        public event PublishRpiSystemStatus PublishStatusEventHandler;

        //publish status information every loggerStatusTimerInterval
        private void OnSystemStatusTimerEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            PublishStatus();
        }

        private void PublishStatus()
        {
            if (PublishStatusEventHandler != null)
            {
                RPiHomeSecurityStatus status;
                status.InAlarm = Alarmed;
                status.Armed = Armed;
                status.Inputs = new List<StatusPinState>();
                foreach (var inputPin in IoBoard.Inputs)
                {
                    status.Inputs.Add(new StatusPinState() {Name = inputPin.Value.Name, State = inputPin.Value.Value});
                }

                status.Outputs = new List<StatusPinState>();
                foreach (var outputPin in IoBoard.Outputs)
                {
                    status.Outputs.Add(new StatusPinState() { Name = outputPin.Value.Name, State = outputPin.Value.Value });
                }

                PublishStatusEventHandler.Invoke(status);
            }
        }

        /// <summary>
        /// get actionlist names
        /// </summary>
        /// <returns></returns>
        public List<String> GetActionLists()
        {
            return Config.ActionLists.Select(actionList => actionList.Key).ToList();
        }
    }
}