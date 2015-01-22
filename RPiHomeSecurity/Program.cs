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
                        log.LogDebugMessage("No config file, creating default. Populate config.cfg with relevant settings");
                        Config.WriteConfigFile(new Config());
                    }
                    else
                    {
                        var config = Config.ReadConfigFile();
                        var alarm = new Alarm(config);
                        while (true)
                        {
                        }
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