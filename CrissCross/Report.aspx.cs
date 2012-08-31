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
using System.Text;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Configuration;
using CrissCross.Code;
using CrissCrossLib;
using log4net;

using Microsoft.Reporting.WebForms;

namespace CrissCross
{
    public partial class Report : System.Web.UI.Page
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Report));

        private string m_reportPath = null;
        private string m_initialParameterString = null;
        private bool m_runningReport = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ReportPath = Request.QueryString["Path"];
            this.InitialParameterString = Request.QueryString["Parameters"];
            if (!this.IsPostBack)
            {
                logger.DebugFormat("Showing report {0} for user {1}", this.ReportPath, User.Identity.Name);
                PrepareReportDefn(this.ReportPath);
            }
        }

        public void PrepareReportDefn(string reportPath)
        {
            var crServices = new CrissCrossLib.CrissCrossServices();
            var crcRepDefn = crServices.GetReportDefn(reportPath, User.Identity.Name);

            if (!String.IsNullOrEmpty(this.InitialParameterString))
            {
                var choiceFactory = new CrcParameterChoiceFactory();
                var choiceCollection = choiceFactory.Create(this.InitialParameterString);
                if (crcRepDefn.HasDependantParameters)
                    crServices.RefreshDependantParameters(crcRepDefn, choiceCollection);
                else
                {
                    var mapResult = crcRepDefn.MapParameterChoices(choiceCollection);
                    if (!mapResult.MappingValid)
                        throw new ApplicationException(String.Format("Could not apply parameters to report {0}. problems: {1}",
                            crcRepDefn.DisplayName, String.Join(", ", mapResult.Complaints.ToArray())));
                }
            }

            uxReportName.Text = crcRepDefn.DisplayName;
            string combinedDesc = BuildCombinedDescription(crcRepDefn);
            if (!string.IsNullOrEmpty(combinedDesc))
                uxReportHint.Text = combinedDesc;
            else
                uxReportDescriptionPanel.Visible = false;
            
            uxReportFolderLink.Text = crcRepDefn.GetReportFolder();
            uxReportFolderLink.NavigateUrl = string.Format("AllCatalog.aspx?Folder={0}",
                Server.UrlEncode(crcRepDefn.GetReportFolder()));
            uxResultsPanel.Visible = false;
            // stash it in session
            Session[crcRepDefn.ReportPath] = crcRepDefn;
         

        }

        private string BuildCombinedDescription(CrcReportDefinition repDefn)
        {
            string combDesc = "";
            if (!string.IsNullOrEmpty(repDefn.Description))
                combDesc = repDefn.Description;
            if (!string.IsNullOrEmpty(repDefn.ReportHint))
            {
                if (combDesc.Length > 0)
                    combDesc += "<br/><br/>";
                combDesc += repDefn.ReportHint;
            }
            return combDesc;
        }
        
        private string GetReportServerUrl()
        {
            return ConfigurationManager.AppSettings["crisscross.ReportServerRootUrl"];
        }

        private CustomReportCredentials GetFixedReportCredentials()
        {
            string username = ConfigurationManager.AppSettings["crisscross.FixedSsrsUsername"];
            string domain =ConfigurationManager.AppSettings["crisscross.FixedSsrsDomain"];
            string password = ConfigurationManager.AppSettings["crisscross.FixedSsrsPassword"];

            if (String.IsNullOrEmpty(username))
                throw new ApplicationException("GetFixedReportCredentials: If crisscross.ImpersonateLoggedOnUser is set to true then crisscross.FixedSsrsUsername etc must be specified");

            return new CustomReportCredentials(username, password, domain);
        }

        private bool GetImpersonateLoggedOnUser()
        {
            return bool.Parse(ConfigurationManager.AppSettings["crisscross.ImpersonateLoggedOnUser"]);
        }

        private string GetFixedSsrsUsername()
        {
            return ConfigurationManager.AppSettings["crisscross.FixedSsrsUsername"]; 
        }

        public string ReportPath
        {
            get { return m_reportPath; }
            set { m_reportPath = value; }
        }

        public string InitialParameterString
        {
            get { return m_initialParameterString; }
            set { m_initialParameterString = value; }
        }

        public bool RunningReport
        {
            get { return m_runningReport; }
            set { m_runningReport = value; }
        }

        public string GetParamDivStyle()
        {
            if (RunningReport)
                return "display:none;";
            else
                return "display:block;";
        }

        public string GetSummaryDivStyle()
        {
            if (RunningReport)
                return "display:block;";
            else
                return "display:none;";
        }

        public bool ShowClientDebugLog
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings["crisscross.ShowClientDebugLog"]);
            }
        }

        protected void uxRunReportButton_Click(object sender, EventArgs e)
        {
            RunReport();
        }

        public void RunReport()
        {
            var crcRepDef = GetReportDefinition(this.ReportPath);
            string paramString = uxHiddenParamString.Text;
            SetReportViewerSize();
            var choiceFactory = new CrcParameterChoiceFactory();
            var choiceCollection = choiceFactory.Create(paramString);
            var mapResult = crcRepDef.MapParameterChoices(choiceCollection);
            if (mapResult.MappingValid)
            {

                InitialiseReportViewer(this.ReportPath);

                logger.DebugFormat("RunReport: ReportPath: {0} User {1}", uxReportViewer.ServerReport.ReportPath, User.Identity.Name);
                var converter = new CrcParameterConverter();
                var ssrsParamList = converter.GetReportParametersForSsrsReportViewer(crcRepDef);
                var userParamList = converter.GetReportParametersForUser(crcRepDef, 15);
                try
                {
                    uxReportViewer.ServerReport.SetParameters(ssrsParamList);
                }
                catch (ReportServerException rse)
                {
                    // check for problem with fixed user
                    if (rse.ErrorCode == "rsAccessDenied" && !this.GetImpersonateLoggedOnUser())
                        throw new ApplicationException(string.Format("ReportViewer is running as fixed user {0} but was passed a report {1} that it could not open",
                            GetFixedSsrsUsername(), this.ReportPath));
                    else
                        throw rse;
                }
                string executionId = uxReportViewer.ServerReport.GetExecutionId();
                logger.DebugFormat("RunReport: running report, executionId is {0}", executionId);
                if (StoreCrissCrossHistory)
                {
                    // log executionid, logged in user and param description 
                    var historyLogger = new CrissCrossLib.History.CrcHistoryLogger();
                    historyLogger.LogToCrissCrossHistory(crcRepDef, User.Identity.Name, executionId);
                }
                uxParamUserDescription.Text = string.Join("<br/>", userParamList.ToArray());
                uxResultsPanel.Visible = true;
                uxParamSummaryPanel.Visible = true;
                this.RunningReport = true;
            }
            else
            {
                // todo - friendly message back to ui
                throw new ApplicationException(string.Format("invalid params - could not map supplied values to definitions for report {0}. complaints: {1}",
                    crcRepDef.DisplayName, string.Join(", ", mapResult.Complaints.ToArray())));
            }
        }

        public void SetReportViewerSize()
        {
            // try and get client viewport size which will have been passed up to server
            int viewportWidth = 0;
            int viewportHeight = 0;
            int.TryParse(uxHiddenViewportWidth.Text, out viewportWidth);
            int.TryParse(uxHiddenViewportHeight.Text, out viewportHeight);
            logger.DebugFormat("SetReportViewerSize: client viewport size reported as: Width {0} Height {1} for User {2}", viewportWidth, viewportHeight, User.Identity.Name);
            // set sensible defaults if nothing came through
            if (viewportWidth == 0)
                viewportWidth = 800;
            if (viewportHeight == 0)
                viewportHeight = 600;
            // make sure not too small
            if (viewportHeight < 300)
                viewportHeight = 300;
            int heightMargin = 130;
            // for narrow widths, reportviewer toolbar will wrap, need to adjust for that
            if (viewportWidth < 892)
                heightMargin = 160;
            // reportviewer width will auto-adjust anyway, but set height here:
            uxReportViewer.Height = viewportHeight - heightMargin;
            
        }

        private void InitialiseReportViewer(string reportPath)
        {
            uxReportViewer.ProcessingMode = ProcessingMode.Remote;
            if (!GetImpersonateLoggedOnUser())
            {
                logger.DebugFormat("Using ReportViewer with fixed user credentials");
                uxReportViewer.ServerReport.ReportServerCredentials = GetFixedReportCredentials();
            }
            else
            {
                logger.DebugFormat("Using ReportViewer with impersonation of logged in user");
            }
 
            uxReportViewer.ServerReport.ReportServerUrl = new Uri(GetReportServerUrl());
            uxReportViewer.ServerReport.ReportPath = reportPath;
            uxReportViewer.ShowParameterPrompts = false;
        }

        public bool StoreCrissCrossHistory
        {
            get
            {
                if (ConfigurationManager.AppSettings["crisscross.StoreCrissCrossHistory"] == null)
                    return false;
                return bool.Parse(ConfigurationManager.AppSettings["crisscross.StoreCrissCrossHistory"]);
            }
        }

        public static CrissCrossLib.CrcReportDefinition GetReportDefinition(string path)
        {
            var crcrRepDef = HttpContext.Current.Session[path] as CrissCrossLib.CrcReportDefinition;
            return crcrRepDef;
        }

        [WebMethod]
        public static string testPageMethod()
        {
            try
            {
                return "page method response";
            }
            catch (Exception e)
            {
                LogAjaxError(e, "testPageMethod");
                throw e;
            }
        }

        [WebMethod]
        public static string getParameterInfo(string path, string paramName)
        {
            try
            {
                var crcRepDef = GetReportDefinition(path);
                logger.DebugFormat("getParameterInfo: Report {0} User {1} requested Param {2}",
                    crcRepDef.DisplayName, HttpContext.Current.User.Identity.Name, paramName);
                var paramDef = crcRepDef.ParameterDefinitions.First(p => p.Name == paramName);
                string json = CrissCrossWebHelper.SerializeObjectIntoJson<CrissCrossLib.CrcParameterDefinition>(paramDef);
                return json;
            }
            catch (Exception e)
            {
                LogAjaxError(e, "getParameterInfo");
                throw e;
            }
        }

        [WebMethod]
        public static string getDependantParameters(string path, string paramName, string paramValue, string[] visibleDependants)
        {
            try
            {
                var crcRepDef = GetReportDefinition(path);
                logger.DebugFormat("getDependantParameters: Report {0} User {1} base param {2} value {3} visible dependants {4}",
                    crcRepDef.DisplayName, HttpContext.Current.User.Identity.Name, paramName, paramValue, string.Join(", ",visibleDependants));
                // map the parameter values onto the report definition
                var choiceFactory = new CrcParameterChoiceFactory();
                var choiceCollection = choiceFactory.Create(paramValue);

                var crServices = new CrissCrossLib.CrissCrossServices();
                crServices.RefreshDependantParameters(crcRepDef, choiceCollection);
                // only return visible dependant params back to ui
                List<CrcParameterDefinition> paramsToReturn = crcRepDef.ParameterDefinitions
                    .Where(p => visibleDependants.Contains(p.Name)).ToList();

                string json = CrissCrossWebHelper.SerializeObjectIntoJson<List<CrcParameterDefinition>>(paramsToReturn);
                return json;
            }
            catch (Exception e)
            {
                LogAjaxError(e, "getDependantParameters");
                throw e;
            }
        }

        [WebMethod]
        public static string getMinimumParametersToDisplay(string path)
        {
            try
            {
                var crcRepDef = GetReportDefinition(path);
                if (crcRepDef == null)
                    throw new ApplicationException(String.Format("Failed to retrieve repdef object for path {0}", path));
                logger.DebugFormat("getMinimumParametersToDisplay: Report {0} User {1}",
                    crcRepDef.DisplayName, HttpContext.Current.User.Identity.Name);
                var ret = new ParametersAndParamListViewModel();
                ret.ParameterDefinitions = crcRepDef.GetMinimumParametersToDisplay();
                ret.AvailableParameters = crcRepDef.GetAvailableParameterNames();
                string json = CrissCrossWebHelper.SerializeObjectIntoJson<ParametersAndParamListViewModel>(ret);
                return json;
            }
            catch (Exception e)
            {
                LogAjaxError(e, "getMinimumParametersToDisplay");
                throw e;
            }
        }

        public static void LogAjaxError(Exception e, string sourceMethod)
        {
            // make a note of the error in log4net, save full details to Elmah
            logger.ErrorFormat("*** Error during Ajax call to {0}. Message: {1}", sourceMethod, e.Message);
            Elmah.ErrorSignal.FromCurrentContext().Raise(e);
        }

        public class ParametersAndParamListViewModel
        {
            public List<CrcParameterDefinition> ParameterDefinitions { get; set; }
            public Dictionary<string,string> AvailableParameters { get; set; }
        }
      

       

    }
}
