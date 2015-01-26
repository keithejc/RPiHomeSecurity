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
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace RpHsWebServiceLib
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single, IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RpHsWebService : IRpHsWebService
    {
        public delegate void RunActionListEventHandler(String actionListName);

        public event RunActionListEventHandler runActionListEventHandler;

        public delegate AlarmStatus GetStatusEventHandler();

        public event GetStatusEventHandler getStatusEventHandler;

        public bool RunActionList(String actionListName)
        {
            if (runActionListEventHandler != null)
            {
                runActionListEventHandler.Invoke(actionListName);
            }
            else
            {
                throw new InvalidOperationException("RunActionListEventHandler Callback not setup in RpHsWebService");
            }

            return true;
        }

        public AlarmStatus GetStatus()
        {
            var ret = new AlarmStatus();

            if (getStatusEventHandler != null)
            {
                ret = getStatusEventHandler.Invoke();
            }
            else
            {
                throw new InvalidOperationException("getStatusEventHandler Callback not setup in RpHsWebService");
            }

            return ret;
        }
    }
}