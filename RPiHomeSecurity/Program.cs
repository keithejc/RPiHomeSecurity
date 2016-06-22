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

namespace RPiHomeSecurity
{
    internal class Program
    {
        private static readonly Mutex Mutex = new Mutex(true, "{1189A51E-3281-43FA-9048-FD7EE0F8F94D}");

        private const String Wampweb = "ws://127.0.0.1:8080/ws";
        private const String Realm = "rpihomesecurity";


        private static void Main(String[] args)
        {
            //only allow one instance of this program to run
            if (Mutex.WaitOne(TimeSpan.Zero, true))
            {
                try
                {
                    var version = System.Reflection.Assembly.GetExecutingAssembly()
                                           .GetName()
                                           .Version
                                           .ToString();
                    Log.LogMessage("RPi HomeSecurity Version: " + version);

                    if (!Config.IsConfigFilePresent())
                    {
                        Log.LogMessage(
                            "No config file, creating default. Populate config.cfg with relevant settings");
                        Config.WriteConfigFile(new Config());
                    }
                    else
                    {
                        var config = Config.ReadConfigFile();
                        var alarm = new Alarm(config);

                        //create wamp client
                        WampBackend.Instance.Init(Wampweb, Realm);

                        //link up web service
                        WampBackend.Instance.RunActionListHandler += alarm.RunActionList;
                        alarm.PublishStatusEventHandler += WampBackend.Instance.PublishRpiSystemStatus;
                        WampBackend.Instance.GetActionListsHandler += alarm.GetActionLists;

                        //try to connect to wamp router every 20 seconds if not already connected
                        var termSignal = new UnixSignal(Signum.SIGINT);
                        var start = DateTime.Now;
                        while (!termSignal.IsSet)
                        {
                            if (!WampBackend.Instance.IsConnected)
                            {
                                var interval = DateTime.Now - start;
                                if (interval.TotalSeconds > 20)
                                {
                                    start = DateTime.Now;
                                    WampBackend.Instance.Init(Wampweb, Realm);
                                }
                            }
                        }

                        WampBackend.Instance.Close();

                    }
                }
                finally
                {
                    Mutex.ReleaseMutex();
                    Log.LogMessage("RPi HomeSecurity exiting");
                }
            }
            else
            {
                Log.LogMessage("HomeSecurity already running, exiting");
            }
        }
    }
}