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
using System.Text;
using log4net;
using rws = CrissCrossLib.ReportWebService;

namespace CrissCrossLib
{
    /// <summary>
    /// Class to handle 'refreshing' Parameter valid values and chosen values
    /// when valid values change due to dependencies
    /// </summary>
    public class CrcParameterRefresher
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CrcParameterRefresher));
        
        public void RefreshParameter(CrcParameterDefinition paramDefn, rws.ReportParameter latestParamDetails)
        {
            logger.DebugFormat("Updating {0} with {1} valid values", paramDefn.Name, 
                (latestParamDetails.ValidValues != null) ? latestParamDetails.ValidValues.Count().ToString() : "null");
            UpdateValidValues(paramDefn, latestParamDetails.ValidValues);
            if (!IsChoiceValid(paramDefn))
            {
                logger.DebugFormat("Param {0} choices are now invalid so applying {1} default values", paramDefn.Name, 
                    (latestParamDetails.DefaultValues != null) ? latestParamDetails.DefaultValues.Count().ToString() : "null");
                UpdateDefaultValues(paramDefn, latestParamDetails.DefaultValues);
            }
        }

        public void UpdateValidValues(CrcParameterDefinition paramDefn, rws.ValidValue[] newValidValues)
        {
            paramDefn.ValidValues.Clear();
            if (newValidValues != null)
            {
                foreach (var valLoop in newValidValues)
                {
                    paramDefn.ValidValues.Add(new CrcValidValue() { Value = valLoop.Value, Label = valLoop.Label });
                }
            }
        }

        public void UpdateDefaultValues(CrcParameterDefinition paramDefn, string[] newDefaultValues)
        {
            var newChoice = new CrcParameterChoice(paramDefn.Name);
            if (newDefaultValues != null)
            {
                foreach (string sloop in newDefaultValues)
                    newChoice.Values.Add(sloop);
            }
            paramDefn.ParameterChoice = newChoice;
        }

        public bool IsChoiceValid(CrcParameterDefinition paramDefn)
        {
            if (paramDefn.ParameterChoice == null)
                return true;
            
            foreach (string valLoop in paramDefn.ParameterChoice.Values)
            {
                var match = paramDefn.ValidValues.FirstOrDefault(vv => vv.Value == valLoop);
                if (match == null)
                    return false;
            }
            return true;
        }

    }
}
