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
using viewer = Microsoft.Reporting.WebForms;
using rws = CrissCrossLib.ReportWebService;

namespace CrissCrossLib
{
    /// <summary>
    /// Given a CrcReportDefinition, the methods of this class convert the Parameter Choices into various formats
    /// (ReportViewer format, user-readable format, SSRS Web Service format, etc)
    /// </summary>
    public class CrcParameterConverter
    {

        public List<viewer.ReportParameter> GetReportParametersForSsrsReportViewer(CrcReportDefinition reptDefn)
        {
            var convertedParams = new List<viewer.ReportParameter>();
            foreach (var paramDefn in reptDefn.ParameterDefinitions)
            {
                if (paramDefn.ParameterChoice != null)
                {
                    var convParam = new viewer.ReportParameter();
                    convParam.Name = paramDefn.Name;
                    if (!string.IsNullOrEmpty(paramDefn.ParameterChoice.SingleValue))
                    {
                        foreach (var valLoop in paramDefn.ParameterChoice.Values)
                            convParam.Values.Add(valLoop);
                    }
                    else
                    {
                        // we want to record a null
                        convParam.Values.Add(null);
                    }
                    convertedParams.Add(convParam);
                }
            }
            return convertedParams;
        }


        public List<rws.ParameterValue> GetParametersValuesForSsrsWebService(CrcReportDefinition reptDefn)
        {
            var convertedParams = new List<rws.ParameterValue>();
            foreach (var paramDefn in reptDefn.ParameterDefinitions)
            {
                if (paramDefn.ParameterChoice != null)
                {
                    if (!string.IsNullOrEmpty(paramDefn.ParameterChoice.SingleValue))
                    {
                        foreach (string valLoop in paramDefn.ParameterChoice.Values)
                        {
                            var convParam = new rws.ParameterValue();
                            convParam.Name = paramDefn.Name;
                            convParam.Value = valLoop;                         
                            convertedParams.Add(convParam);
                        }
                    }
                }
            }
            return convertedParams;
        }


        public List<string> GetReportParametersForUser(CrcReportDefinition reptDefn)
        {
            return GetReportParametersForUser(reptDefn, 0);
        }

        /// <summary>
        /// Gets a user-friendly list of report parameters
        /// </summary>
        /// <param name="reptDefn"></param>
        /// <param name="truncateListLimit">for multipicks, the max number of items to list (0 for all items)</param>
        /// <returns></returns>
        public List<string> GetReportParametersForUser(CrcReportDefinition reptDefn, int truncateListLimit)
        {
            var userParams = new List<string>();
            foreach (var paramDefn in reptDefn.ParameterDefinitions)
            {
                if (!paramDefn.IsEmptyEquivalent)
                {
                    string valueString = "";
                    if (paramDefn.ParameterType == CrcParameterType.Date)
                    {
                        valueString = DateTime.Parse(paramDefn.ParameterChoice.SingleValue).ToString("dd/MMM/yyyy");
                    }
                    else if (paramDefn.ParameterType == CrcParameterType.Boolean)
                    {
                        valueString = paramDefn.ParameterChoice.SingleValue;
                    }
                    else if (paramDefn.ParameterType == CrcParameterType.Text)
                    {
                        valueString = paramDefn.ParameterChoice.SingleValue;
                    }
                    else
                    {
                        valueString = SelectListAsString(paramDefn, truncateListLimit);
                    }
                    userParams.Add(String.Format("{0}: {1}", paramDefn.DisplayName, valueString));
                
                }
            }
            return userParams;
        }

        private string SelectListAsString(CrcParameterDefinition paramDefn, int truncateListLimit)
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (var valLoop in paramDefn.ParameterChoice.Values)
            {
                var vvMatch = paramDefn.ValidValues.FirstOrDefault(vv => vv.Value == valLoop);
                if (vvMatch == null)
                    throw new ApplicationException(string.Format("Param {0}: {1} is not a valid value", paramDefn.Name, valLoop));

                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(vvMatch.Label);
                count += 1;
                if (truncateListLimit > 0 && count > truncateListLimit)
                {
                    sb.AppendFormat(" ... (and {0} others)", paramDefn.ParameterChoice.Values.Count() - count);
                    break;
                }
            }
            return sb.ToString();
        }
    }
}
