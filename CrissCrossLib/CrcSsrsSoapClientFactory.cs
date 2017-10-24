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
using System.Text;
using System.Configuration;
using System.Net;
using log4net;
using rws = CrissCrossLib.ReportWebService;

namespace CrissCrossLib
{
    public class CrcSsrsSoapClientFactory
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CrcSsrsSoapClientFactory));
        

        public virtual rws.ReportingService2005Soap MakeSsrsSoapClient()
        {
            rws.ReportingService2005SoapClient rService = new rws.ReportingService2005SoapClient("ReportingService2005Soap", GetWebServiceUrl());
            if (GetImpersonateLoggedOnUser())
            {
                logger.DebugFormat("Making Ssrs Soap client with impersonation of logged in user");
                rService.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
                rService.ChannelFactory.Credentials.Windows.ClientCredential = CredentialCache.DefaultNetworkCredentials;
            }
            else
            {
                // NB: for this to work properly, ApplicationPool account should match crisscross.FixedSsrsUsername account in web.config
                logger.DebugFormat("Making Ssrs Soap client with default network credentials (ie Application Pool credentials)");
                rService.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
                rService.ChannelFactory.Credentials.Windows.ClientCredential = CredentialCache.DefaultNetworkCredentials;
            }

            return (rws.ReportingService2005Soap) rService;
        }

        private bool GetImpersonateLoggedOnUser()
        {
            return bool.Parse(ConfigurationManager.AppSettings["crisscross.ImpersonateLoggedOnUser"]);
        }

        private string GetWebServiceUrl()
        {
            return ConfigurationManager.AppSettings["crisscross.ReportServerWebServiceUrl"];
        }

        

    }
}
