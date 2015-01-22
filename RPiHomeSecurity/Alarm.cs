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
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Web;
using RpHsWebServiceLib;
using System.Timers;

namespace RPiHomeSecurity
{
    public class Alarm
    {

        public bool Armed{ get; set;}
        public bool Alarmed { get; set; }
        public bool Warning { get; set; }

        private GpioController ioBoard;

        public GpioController IoBoard { get; set; }

        private Config config;

        private WebServiceHost _serviceHost;
        private RpHsWebService rpHsWebService;
        private System.Timers.Timer warningTimer;

        //setup the alarm system
        public Alarm(Config configuration)
        {
            Warning = false;
            Alarmed = false;

            warningTimer = new System.Timers.Timer();
            warningTimer.Elapsed += WarningTimerElapsed;

            //get configuration
            config = configuration;

            ioBoard = new GpioController(config);
            ioBoard.inputChangedEventHandler += new InputChangedEventHandler(InputHandler);

            SendEmail("Start", "");
            log.LogDebugMessage("Rpi Home Security Ready...");

            //start web service
            var ipAddress = Utils.GetIpAddress();
            if (ipAddress != IPAddress.None)
            {
                rpHsWebService = new RpHsWebService();
                rpHsWebService.getStatusEventHandler += new RpHsWebService.GetStatusEventHandler(GetStatus);
                rpHsWebService.setArmedEventHandler += new RpHsWebService.SetArmedEventHandler(Arm);
                log.LogDebugMessage("Web Service binding to: " + ipAddress.ToString() + ":" + config.WebInterfacePort);

                _serviceHost = new WebServiceHost(rpHsWebService, new Uri("http://" + ipAddress.ToString() + ":" + config.WebInterfacePort + "/RpHsWebService"));
                _serviceHost.Open();
            }
            else
            {
                log.LogError("Failed to get ip address of this device, no web interface available");
            }

            Arm(true);

        }

        ~Alarm()
        {
            _serviceHost.Close();
        }

        //callback for the web service
        public bool GetArmed()
        {
            return Armed;
        }

        //callback for the web service
        private AlarmStatus GetStatus()
        {
            return new AlarmStatus() { Armed = this.Armed, InAlarm = this.Alarmed };
        }

        //send an email
        public void SendEmail(String subject, String body)
        {
            try
            {
                log.LogDebugMessage("Send email: '" + subject + "' to " + config.EmailAddress);

                String toAddress = config.EmailAddress;
                MailAddress from = new MailAddress(config.SmtpFromAddress);
                MailAddress to = new MailAddress(toAddress);

                string fromPassword = config.SmtpPassword;
                var smtpClient = new SmtpClient
                {
                    Host = config.SmtpServer,
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(from.Address, fromPassword)
                };
                ServicePointManager.ServerCertificateValidationCallback =
                    delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    { return true; };

                MailMessage msg = new MailMessage(from, to)
                {
                    Subject = subject,
                    Body = DateTime.Now.ToLongDateString() + Environment.NewLine +
                    DateTime.Now.ToLongTimeString() + Environment.NewLine +
                    config.EmailBody + Environment.NewLine +
                    body
                };

                smtpClient.SendCompleted += (s, e) =>
                {
                    smtpClient.Dispose();
                    msg.Dispose();
                };

                smtpClient.SendAsync(msg, null);
            }
            catch (Exception e)
            {
                log.LogError("Error sending email: " + e.ToString());
            }
        }

        //in the warning state, if not disarmed within 
        //warningtime then go into alarm
        private void SetWarning(bool isOn)
        {
            Warning = isOn;

            if (isOn)
            {
                int numToggles = config.WarningDuration / (config.WarningToggleOn + config.WarningToggleOff);
                ioBoard.Toggle("Siren", config.WarningToggleOn, config.WarningToggleOff, numToggles);

                warningTimer.Interval = config.WarningDuration;
                warningTimer.Start();

            }
            else
            {
                ioBoard.TurnOffOutput("Siren");

                warningTimer.Stop();
            }

        }

        //waring timer has elapsed - go into alarm
        private void WarningTimerElapsed(object sender, ElapsedEventArgs e)
        {
            SetWarning(false);
            AlarmOn(true);
        }


        //go into alarm state
        private void AlarmOn(bool isOn)
        {
            Alarmed = isOn;
            if (isOn)
            {
                ioBoard.TurnOnOutput("Siren", config.AlarmSirenDuration);
                SendEmail("Alarm", "");
            }
            else
            {
                ioBoard.TurnOffOutput("Siren");
            }
        }

        //arm/disarm
        private void Arm(bool armed)
        {
            SetWarning(false);
            AlarmOn(false);

            Armed = armed;
            if (armed)
            {

                //check door isn't already on
                if (ioBoard.Inputs["Door"].Value == PinState.High)
                {
                    //sound warning if it is
                    ioBoard.TurnOnOutput("Siren", config.WarningDuration);

                    log.LogDebugMessage("Armed but door already open");
                }
                else
                {
                    ioBoard.TurnOnOutput("Light", 1000);
                    ioBoard.Toggle("Siren", config.ChimeDuration, 200, 10);

                    log.LogDebugMessage("Armed");
                }
            }
            else
            {
                log.LogDebugMessage("DisArmed");
                ioBoard.TurnOnOutput("Light", 5000);

                ioBoard.Toggle("Siren", config.ChimeDuration, 500, 2);
            }
        }


        //does all the handling of input changes
        private void InputHandler(IInputPin inputPin)
        {
            try
            {
                var pinState = inputPin.Value;
                log.LogDebugMessage(inputPin.Name);

                //alarm, siren, alert user
                if (inputPin.Name == "Door" && pinState == PinState.High && Armed == true && Alarmed == false && Warning == false)
                {
                    SetWarning(true);
                }

                //handle - turn on light, chirp siren, alert user
                if (inputPin.Name == "Handle" && pinState == PinState.High)
                {
                    HandleTurned();
                }

                //arm
                if (inputPin.Name == "Arm" && pinState == PinState.Low)
                {
                    Arm(true);
                }

                //disarm
                if (inputPin.Name == "DisArm" && pinState == PinState.Low)
                {
                    Arm(false);
                }

                //power supply fail, alert user
                if (inputPin.Name == "MainsOk" && pinState == PinState.High)
                {
                    SendEmail("PowerFail", "");
                }
            }
            catch (Exception e)
            {
                log.LogError("Alarm loop error, ended: " + e.ToString());
                SendEmail("Alarm loop error", "Error details " + e.ToString());
            }
        }

        //handle turned- sound warning and email user
        private void HandleTurned()
        {
            ioBoard.TurnOnOutput("Light", config.WarningDuration);

            if (Armed && Alarmed == false && Warning == false)
            {
                ioBoard.Toggle("Siren", config.ChimeDuration, 500, 5);
                SendEmail("Handle", "");
            }
        }
    }
}