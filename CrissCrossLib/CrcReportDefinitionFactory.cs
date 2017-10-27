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
using CrissCrossLib.Configuration;
using rws = CrissCrossLib.ReportWebService;
using Microsoft.Reporting.WebForms;
using log4net;

namespace CrissCrossLib
{
    public class CrcReportDefinitionFactory
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CrcReportDefinitionFactory));

        // In ReportService2005 endpoint, parameter types were an enum
        // In ReportService2010 endpoint, types are represented by strings
        // the ws method ListParameterTypes() is supposed to return these types
        // but in practice they appear to be fixed anyway.
        // see https://msdn.microsoft.com/en-us/library/reportservice2010.reportingservice2010.listparametertypes.aspx
        // Hence this static class operates somwhat like an Enum,
        // but with String values
        public static class ReportServiceParameterTypes
        {
            public static readonly String Boolean = "Boolean";
            public static readonly String DateTime = "DateTime";
            public static readonly String Float = "Float";
            public static readonly String Integer = "Integer";
            public static readonly String String = "String";
        }

        public CrcReportDefinition Create(string reportPath, rws.ItemParameter[] wsReportParameters)
        {
            return Create(reportPath, null, wsReportParameters, null);
        }

        public CrcReportDefinition Create(string reportPath, rws.ItemParameter[] wsReportParameters, CrcExtraConfiguration extraConfig)
        {
            return Create(reportPath, null, wsReportParameters, extraConfig);
        }

        public CrcReportDefinition Create(string reportPath, rws.CatalogItem reportCatItem, rws.ItemParameter[] wsReportParameters, CrcExtraConfiguration extraConfig)
        {
            var repDef = new CrcReportDefinition();
            repDef.ReportPath = reportPath;
            if (reportCatItem != null)
            {
                repDef.DisplayName = reportCatItem.Name;
                repDef.Description = reportCatItem.Description;
            }
            if (string.IsNullOrEmpty(repDef.DisplayName))
            {
                repDef.DisplayName = CrcReportDefinition.ReportNameFromPath(repDef.ReportPath);
            }

            CrcReportConfig repConfig = null;
            if (extraConfig != null)
                repConfig = extraConfig.GetReportConfig(repDef.ReportPath);

            if (repConfig != null)
                repDef.ReportHint = repConfig.ReportHint;

            AddParameterDefinitions(wsReportParameters, repDef, extraConfig, repConfig);

            bool readSsrsDependantParams = false;
            if (extraConfig == null || !extraConfig.IgnoreSsrsParameterDependencies)
                readSsrsDependantParams = true;
            if (repConfig != null && repConfig.DependantParamsSpecified)
                readSsrsDependantParams = false;
            if (readSsrsDependantParams)
                AddSsrsDependentParams(wsReportParameters, repDef);

            CrossReferenceDependantParameters(repDef);
            ApplyParameterDefaults(wsReportParameters, repDef);
            CheckRequiredFromUser(repDef);

            return repDef;
        }

        public void AddParameterDefinitions(rws.ItemParameter[] wsReportParameters, CrcReportDefinition repDef, CrcExtraConfiguration extraConfig, CrcReportConfig reportConfig)
        {
            List<string> showByDefault = new List<string>();
            if (reportConfig != null)
                showByDefault = reportConfig.GetParamsToShowByDefault();
            List<string> defaultEmptyEquivalents = new List<string>();
            if (extraConfig != null && extraConfig.DefaultEmptyEquivalentValues != null)
                defaultEmptyEquivalents = extraConfig.DefaultEmptyEquivalentValues;
            
            foreach (var paramLoop in wsReportParameters)
            {
                CrcReportConfig.CrcParamConfig paramConfig = null;
                // get extra config for parameter, if there is any
                if (reportConfig != null)
                    paramConfig = reportConfig.CrcParamConfigs.FirstOrDefault(p => p.ParamName == paramLoop.Name);

                var crcParam = new CrcParameterDefinition();
                crcParam.Name = paramLoop.Name;
                crcParam.id = "param_" + paramLoop.Name.Replace(" ", "_");
                crcParam.AllowNull = paramLoop.Nullable;
                crcParam.AllowBlank = paramLoop.AllowBlank;
                
                if (string.IsNullOrEmpty(paramLoop.Prompt))
                {
                    // if Prompt is null or empty, it means Parameter is 'Hidden' in SSRS
                    crcParam.Hidden = true;
                    crcParam.DisplayName = paramLoop.Name;
                }
                else
                {
                    crcParam.DisplayName = paramLoop.Prompt;
                }
                if (string.IsNullOrEmpty(crcParam.DisplayName))
                    crcParam.DisplayName = crcParam.Name;
                // if PromptUser is false then Parameter is 'Internal' in SSRS
                if (!paramLoop.PromptUser)
                    crcParam.Hidden = true;
                var a = new rws.ItemParameter();
               
                if (paramLoop.ParameterTypeName != null && paramLoop.ParameterTypeName.Equals(ReportServiceParameterTypes.DateTime))
                    crcParam.ParameterType = CrcParameterType.Date;
                else if ((paramLoop.ValidValues != null && paramLoop.ValidValues.Count() > 0)
                    || paramLoop.ValidValuesQueryBased)
                {
                    if (paramLoop.MultiValue)
                        crcParam.ParameterType = CrcParameterType.MultiSelect;
                    else
                        crcParam.ParameterType = CrcParameterType.Select;
                }
                else if (paramLoop.ParameterTypeName != null && paramLoop.ParameterTypeName.Equals(ReportServiceParameterTypes.Boolean))
                    crcParam.ParameterType = CrcParameterType.Boolean;
                else
                    crcParam.ParameterType = CrcParameterType.Text;


                if (paramLoop.ValidValues != null)
                {
                    foreach (var valLoop in paramLoop.ValidValues)
                    {
                        crcParam.ValidValues.Add(new CrcValidValue() { Value = valLoop.Value, Label = valLoop.Label });
                    }
                    if (crcParam.ValidValues.Count() > 10)
                        crcParam.AllowListSearch = true;
                }

               
                // check config for dependencies
                if (paramConfig != null && paramConfig.DependantParams != null && paramConfig.DependantParams.Count() > 0)
                {
                    logger.DebugFormat("Param {0} has extraconfig dependancies: {1}",
                        paramLoop.Name, string.Join(", ", paramConfig.DependantParams.ToArray()));
                    foreach (string dpname in paramConfig.DependantParams)
                        crcParam.DependantParameterNames.Add(dpname);
                }
                


                foreach (string sloop in defaultEmptyEquivalents)
                {
                    AddEmptyEquivalent(crcParam, sloop);
                }

                // check for specific empty equivalents for this parameter
                if (paramConfig != null && paramConfig.EmptyEquivalentValues != null)
                {
                    foreach (string sloop in paramConfig.EmptyEquivalentValues)
                    {
                        AddEmptyEquivalent(crcParam, sloop);
                    }
                }



                if (showByDefault.Contains(crcParam.Name))
                    crcParam.AlwaysShow = true;

                repDef.ParameterDefinitions.Add(crcParam);
            }
        }

        private void AddEmptyEquivalent(CrcParameterDefinition crcParam, string emptyEquiv)
        {
            if (crcParam.ParameterType == CrcParameterType.MultiSelect ||
                crcParam.ParameterType == CrcParameterType.Select)
            {
                // add if valid value
                if (crcParam.ValidValues.FirstOrDefault(vv => vv.Value == emptyEquiv) != null)
                    crcParam.EmptyEquivalentValues.Add(emptyEquiv);
            }
            else
                // just add
                crcParam.EmptyEquivalentValues.Add(emptyEquiv);
        }

        /// <summary>
        /// Reads the dependent param info from the ssrs web service params and
        /// converts into dependAnt params (i.e. opposite way round)
        /// </summary>
        public void AddSsrsDependentParams(rws.ItemParameter[] wsReportParameters, CrcReportDefinition repDef)
        {
            foreach (var paramLoop in wsReportParameters)
            {
                if (paramLoop.Dependencies != null)
                {
                    logger.DebugFormat("AddSsrsDependantParams: Param {0} has dependencies {1} according to web service", paramLoop.Name, string.Join(", ", paramLoop.Dependencies));
                    foreach (string dependentParam in paramLoop.Dependencies)
                    {
                        var match = repDef.ParameterDefinitions.FirstOrDefault(p => p.Name == dependentParam);
                        if (match == null)
                            throw new ApplicationException(String.Format("Ssrs says {0} has Dependent {1} but could not find CrcParameterDefintiion {1} during make stage",
                                paramLoop.Name, dependentParam));

                        match.DependantParameterNames.Add(paramLoop.Name);
                    }
                }
            }
        }

        public void CrossReferenceDependantParameters(CrcReportDefinition repDef)
        {
            foreach (var crParamLoop in repDef.ParameterDefinitions)
            {
                foreach (var dependantName in crParamLoop.DependantParameterNames)
                {
                    var match = repDef.ParameterDefinitions.FirstOrDefault(p => p.Name == dependantName);
                    if (match == null)
                    {
                        logger.DebugFormat("Report {0} Param {1} specified Dependant Param {2} but cannot find it",
                            repDef.DisplayName, crParamLoop.Name, dependantName);
                        continue;
                    }
                    logger.DebugFormat("Report {0} Param {1} has Dependant ID {2} - found",
                            repDef.DisplayName, crParamLoop.Name, match.id);
                    crParamLoop.DependantParameterIds.Add(match.id);
                }
            }
        }

        public void ApplyParameterDefaults(rws.ItemParameter[] wsReportParameters, CrcReportDefinition repDef)
        {
            // first build up a choicecollection from the defaults
            var choiceCol = new CrcParameterChoiceCollection();
            foreach (var paramLoop in wsReportParameters)
            {
                if (paramLoop.DefaultValues != null)
                {
                    var crcChoice = new CrcParameterChoice(paramLoop.Name);
                    foreach (string valString in paramLoop.DefaultValues)
                        crcChoice.Values.Add(valString);

                    choiceCol.ParameterChoiceList.Add(crcChoice);
                }
            }
            // now apply it to the repdef
            var mapper = new CrcParameterChoiceMapper();
            var mapResult = mapper.MapParameterChoices(repDef, choiceCol);
            if (!mapResult.MappingValid)
                throw new ApplicationException(string.Format("Could not map report defaults for report {0}. Problems: {1}",
                    repDef.DisplayName, string.Join(", ", mapResult.Complaints.ToArray())));
        }

        public void CheckRequiredFromUser(CrcReportDefinition repDef)
        {
            foreach (var crcParam in repDef.ParameterDefinitions)
            {
                bool hasDefault = (crcParam.ParameterChoice != null && crcParam.ParameterChoice.Values.Count() > 0);
                bool defaultIsNull = false;
                bool defaultIsBlank = false;
                if (hasDefault)
                {
                    defaultIsNull = crcParam.ParameterChoice.Values.Exists(v => v == null);
                    defaultIsBlank = crcParam.ParameterChoice.Values.Exists(v => v == string.Empty);
                }
                crcParam.RequiredFromUser = false;
                if (!crcParam.AllowNull && (!hasDefault || hasDefault && defaultIsNull))
                    crcParam.RequiredFromUser = true;
                
            }
        }

        
    }
}
