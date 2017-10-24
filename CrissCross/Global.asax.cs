// CrissCross - alternative user interface for running SSRS reports
// Copyright (C) 2011-2017 Ian Finch
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using log4net;

namespace CrissCross
{
    public class Global : System.Web.HttpApplication
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Global));

        protected void Application_Start(object sender, EventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            logger.Debug("Application_Start event");
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            if (logger.IsDebugEnabled)
            {
                string username = (User.Identity != null) ? User.Identity.Name : "";
                string browser = "";
                if (Request.Browser != null)
                {
                    string ieCompatibility = null;
                    if (Request.Browser.Browser == "IE" && Request.Browser.MajorVersion == 7)
                    {
                        // check whether its IE8 pretending to be IE7
                        if (Request.UserAgent.Contains("Trident/4.0"))
                            ieCompatibility = "(IE8 in compatibility mode)";
                    }
                    browser = string.Format("{0} {1} ({2}.{3}) {4}",
                        Request.Browser.Browser, Request.Browser.Version, Request.Browser.MajorVersion,
                        Request.Browser.MinorVersion, ieCompatibility);
                }
                logger.DebugFormat("Session_Start fired for Request {0} IP {1} Username {2} Browser {3}", Request.Url.ToString(), Request.UserHostAddress, username, browser);
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            Exception inner = ex.InnerException;
            // make a note of the exception in log4net, but leave the full details to Elmah
            logger.ErrorFormat("*** Application_Error caught exception. See Elmah error log for details. Message: {0} Inner: {1}", ex.Message,
                (inner != null) ? inner.Message:"None");
        }

        protected void Session_End(object sender, EventArgs e)
        {
            logger.DebugFormat("Session_End fired");
        }

        protected void Application_End(object sender, EventArgs e)
        {
            logger.Debug("Application_End event");
        }
    }
}