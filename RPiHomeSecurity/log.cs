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
using System.IO;

namespace RPiHomeSecurity
{

    public class Log
    {
        private  static readonly Object LogFileLock = new Object();

        public static void LogError(String err)
        {
            LogMessage("Error: " + err);
        }

        public static void LogMessage(String msg)
        {
            try
            {
                lock (LogFileLock)
                {
                    var fileLoc = AppDomain.CurrentDomain.BaseDirectory;
                    var logfile = Path.Combine(fileLoc, "log.txt");

                    Console.WriteLine(msg);
                    File.AppendAllText(logfile, DateTime.Now + "," + msg + Environment.NewLine);
                }
            }
            catch (Exception)
            {
            }
        }

    }
}