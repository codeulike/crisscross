using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using log4net;

namespace CrissCrossLib.History
{
    public class CrcHistoryLogger
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CrcHistoryLogger));
        

        public void LogToCrissCrossHistory(CrcReportDefinition repDef, string username, string executionId)
        {

            string crissCrossInstance = GetAppRootUrl(false);
            string reportPath = repDef.ReportPath;
            var converter = new CrcParameterConverter();
            List<string> parametersListForUser = converter.GetReportParametersForUser(repDef, 0);
            string parametersForUser = string.Join(Environment.NewLine, parametersListForUser.ToArray());

            logger.DebugFormat("Logging report run to CrissCrossExecutionLog - executionid {0}", executionId);

            string sql = "insert into CrissCrossExecutionLog(ExecutionId, CrissCrossInstance, ReportPath, UserName, ParametersForUser) "
                + "values(@ExecutionId, @CrissCrossInstance, @ReportPath, @UserName, @ParametersForUser)";
            using (SqlConnection conn = new SqlConnection(GetCrissCrossHistoryConnectionString()))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.Parameters.Add("@ExecutionId", SqlDbType.NVarChar, 128).Value = executionId;
                comm.Parameters.Add("@CrissCrossInstance", SqlDbType.NVarChar, 250).Value = crissCrossInstance;
                comm.Parameters.Add("@ReportPath", SqlDbType.NVarChar, 850).Value = reportPath;
                comm.Parameters.Add("@UserName", SqlDbType.NVarChar, 520).Value = username;
                comm.Parameters.Add("@ParametersForUser", SqlDbType.NVarChar).Value = parametersForUser;
                comm.ExecuteNonQuery();
            }
            
        }

        private string GetCrissCrossHistoryConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["CrissCrossHistoryDb"].ConnectionString;
        }

        public static string GetAppRootUrl(bool endSlash)
        {
            string host = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string appRootUrl = HttpContext.Current.Request.ApplicationPath;
            if (!appRootUrl.EndsWith("/")) //a virtual
            {
                appRootUrl += "/";
            }
            if (!endSlash)
            {
                appRootUrl = appRootUrl.Substring(0, appRootUrl.Length - 1);
            }
            return host + appRootUrl;
        }
    }
}
