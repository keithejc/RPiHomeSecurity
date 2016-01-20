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
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Web;
using System.Timers;
using RPiHomeSecurity.wamp;

namespace RPiHomeSecurity
{
    public class Alarm
    {
        public bool Armed { get; set; }

        public bool Alarmed { get; set; }

        public IoController IoBoard { get; set; }

        public Config Config { get; set; }

        private System.Timers.Timer loggerStatusTimer;
        private const double statusTimerInterval = 1000;

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

            //run the startup actions
            RunActionList(Config.StartupActionListName);
            log.LogDebugMessage("Rpi Home Security Ready...");

            loggerStatusTimer = new System.Timers.Timer(statusTimerInterval);
            loggerStatusTimer.AutoReset = true;
            loggerStatusTimer.Elapsed += OnSystemStatusTimerEvent;

        }

        ~Alarm()
        {
        }

        //callback for the web service

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
            List<Action> actions = Config.ActionLists[actionList];
            if (actions != null)
            {
                log.LogDebugMessage("Running action list: " + actionList);

                foreach (var action in actions)
                {
                    action.RunAction(this);
                }
            }
        }

        public delegate void PublishRpiSystemStatus(RPiHomeSecurityStatus status);
        public event PublishRpiSystemStatus PublishStatusEventHandler;

        //publish status information at least every loggerStatusTimerInterval
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
            }
        }


    }
}