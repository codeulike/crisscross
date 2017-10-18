// CrissCross - alternative user interface for running SSRS reports
// Copyright (C) 2011 Ian Richardson
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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Configuration;
using System.Web.Configuration;

namespace CrissCross
{
    public partial class About : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                ShowAboutInfo();

            }
        }

        private void ShowAboutInfo()
        {
            string appTitle = Master.AppTitle;
            uxAppTitle.Text = appTitle;
            // need a bit of explanation if we're not using the default name
            if (appTitle != "CrissCross")
                uxNameExplanation.Text = String.Format("<p>{0} is an installation of <b>CrissCross</b>.</p>", appTitle);

            Assembly current = Assembly.GetExecutingAssembly();
            uxVersionNo.Text = current.GetName().Version.ToString();
            uxReportViewerUrl.Text = ConfigurationManager.AppSettings["crisscross.ReportServerRootUrl"];
            uxReportWebServiceUrl.Text = ConfigurationManager.AppSettings["crisscross.ReportServerWebServiceUrl"];
            uxReportManagerUrl.Text = uxReportViewerUrl.Text.ToLower().Replace("reportserver", "reports");
            uxReportManagerUrl.NavigateUrl = uxReportManagerUrl.Text;
            bool crcimpersonate = bool.Parse(ConfigurationManager.AppSettings["crisscross.ImpersonateLoggedOnUser"]);
            if (crcimpersonate)
            {
                uxCrissCrossImpersonationMode.Text = "on";
                uxCrissCrossFixedUserPanel.Visible = false;
            }
            else
            {
                uxCrissCrossImpersonationMode.Text = "off";
                uxCrissCrossFixedUserPanel.Visible = true;
                uxCrissCrossFixedUser.Text = ConfigurationManager.AppSettings["crisscross.FixedSsrsUsername"];
            }
            bool aspimpersonate = GetAspNetImpersonation();
            if (aspimpersonate)
            {
                uxAspNetImpersonationMode.Text = "on";
                uxAspNetFixedUserPanel.Visible = false;
            }
            else
            {
                uxAspNetImpersonationMode.Text = "off";
                uxAspNetFixedUserPanel.Visible = true;
                try
                {
                    var runningUser = System.Security.Principal.WindowsIdentity.GetCurrent().User;
                    var runningUserName = runningUser.Translate(typeof(System.Security.Principal.NTAccount)).Value;
                    uxAspNetFixedUser.Text = runningUserName;
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    uxAspNetFixedUser.Text = "(could not determine)";
                }
            }
            if (crcimpersonate != aspimpersonate)
            {
                uxWarningPanel.Visible = true;
                uxWarningMessage.Text = "Warning: CrissCross Impersonate (crisscross.ImpersonateLoggedOnUser in appSettings) should match Asp.Net Impersonate (identity impersonate setting)";
            }
            uxCurrentUser.Text = User.Identity.Name.ToString();
            if (Request.IsLocal)
                uxLocalOnlyPanel.Visible = true;
        }

        private bool GetAspNetImpersonation()
        {

            Configuration configurationFile = WebConfigurationManager.OpenWebConfiguration("~/web.config");
            IdentitySection configIdentity =
                configurationFile.GetSection("system.web/identity") as IdentitySection;
            return configIdentity.Impersonate;
        }
    }
}
