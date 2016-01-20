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

using System.Threading;
using Mono.Unix;
using Mono.Unix.Native;
using RPiHomeSecurity.wamp;
using WampSharp.Logging;

namespace RPiHomeSecurity
{
    internal class Program
    {
        private static Mutex mutex = new Mutex(true, "{1189A51E-3281-43FA-9048-FD7EE0F8F94D}");

        private static void Main(string[] args)
        {
            //only allow one instance of this program to run
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                try
                {
                    string version = System.Reflection.Assembly.GetExecutingAssembly()
                                           .GetName()
                                           .Version
                                           .ToString();
                    log.LogDebugMessage("RPi HomeSecurity Version: " + version);

                    if (!Config.IsConfigFilePresent())
                    {
                        log.LogDebugMessage(
                            "No config file, creating default. Populate config.cfg with relevant settings");
                        Config.WriteConfigFile(new Config());
                    }
                    else
                    {
                        var config = Config.ReadConfigFile();
                        var alarm = new Alarm(config);

                        //create wamp client
                        string wampweb = "ws://127.0.0.1:8080/ws";
                        string realm = "rpihomesecurity";
                        WampBackend.Instance.Init(wampweb, realm);

                        //link up web service
                        WampBackend.Instance.RunActionListHandler += alarm.RunActionList;

                        alarm.PublishStatusEventHandler += WampBackend.Instance.PublishRpiSystemStatus;

                        //try to connect to wamp router every 20 seconds if not already connected
                        UnixSignal TERMSignal = new UnixSignal(Signum.SIGINT);
                        DateTime start = DateTime.Now;
                        while (!TERMSignal.IsSet)
                        {
                            if (!WampBackend.Instance.IsConnected)
                            {
                                var interval = DateTime.Now - start;
                                if (interval.TotalSeconds > 20)
                                {
                                    start = DateTime.Now;
                                    WampBackend.Instance.Init(wampweb, realm);
                                }
                            }
                        }

                        WampBackend.Instance.Close();

                    }
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            else
            {
                log.LogDebugMessage("HomeSecurity already running, exiting");
            }
        }
    }
}