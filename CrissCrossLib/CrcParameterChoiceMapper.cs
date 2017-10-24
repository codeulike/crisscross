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
using System.Globalization;
using System.Configuration;

namespace CrissCrossLib
{
    public class CrcParameterChoiceMapper
    {

        /// <summary>
        /// applies the specified CrcParameterChoiceCollection to the specified report definition
        /// will set the ParameterChoice property of matching ParameterDefinitions as required
        /// </summary>
        /// <param name="repDefn"></param>
        /// <param name="paramChoices"></param>
        /// <returns>ParameterMapResult - if MappingValid is false, see Complaints collection</returns>
        public ParameterMapResult MapParameterChoices(CrcReportDefinition repDefn, CrcParameterChoiceCollection paramChoices)
        {
            var res = new ParameterMapResult();
            res.Complaints = new List<string>();
            string cultureForDateParsing = GetCultureForDateParsing();
            foreach (CrcParameterChoice choiceLoop in paramChoices.ParameterChoiceList)
            {
                var defnMatch = repDefn.ParameterDefinitions.FirstOrDefault(pd => pd.Name == choiceLoop.Name);
                if (defnMatch == null)
                {
                    res.Complaints.Add(String.Format("Param {0} not found", choiceLoop.Name));
                    continue;
                }
                if (defnMatch.ParameterType == CrcParameterType.Date)
                {
                    if (choiceLoop.SingleValue == null)
                    {
                        defnMatch.ParameterChoice = choiceLoop.DeepClone();
                        continue;
                    }

                    var parseResult = ParseDateStringVariousWays(choiceLoop.SingleValue, cultureForDateParsing);
                    if (parseResult.CouldParse)
                    {
                        var parsedChoice = new CrcParameterChoice(choiceLoop.Name);
                        parsedChoice.SingleValue = parseResult.DateTime.ToString("yyyy-MM-dd");
                        defnMatch.ParameterChoice = parsedChoice;
                        continue;
                    }
                    res.Complaints.Add(String.Format("Could not parse date {0} into Param {1}",
                        choiceLoop.SingleValue, choiceLoop.Name));
                }
                else if (defnMatch.ParameterType == CrcParameterType.Boolean)
                {
                    if (string.IsNullOrEmpty(choiceLoop.SingleValue) || choiceLoop.SingleValue == "True"
                        || choiceLoop.SingleValue == "False")
                    {
                        defnMatch.ParameterChoice = choiceLoop.DeepClone();
                        continue;
                    }
                    res.Complaints.Add(String.Format("Could not parse boolean {0} into Param {1}",
                        choiceLoop.SingleValue, choiceLoop.Name));
                }
                else if (defnMatch.ParameterType == CrcParameterType.Text)
                {
                    defnMatch.ParameterChoice = choiceLoop.DeepClone();
                    continue;
                }
                else if (defnMatch.ValidValues.Count() > 0)
                {
                    bool valuesOK = true;
                    foreach (string chosenValue in choiceLoop.Values)
                    {
                        if (defnMatch.ValidValues.FirstOrDefault(vv => vv.Value == chosenValue) == null)
                        {
                            res.Complaints.Add(String.Format("Could not apply value {0} into Param {1} because it is not a valid option",
                             chosenValue, choiceLoop.Name));
                            valuesOK = false;
                        }

                    }
                    if (valuesOK)
                    {
                        defnMatch.ParameterChoice = choiceLoop.DeepClone();
                        continue;
                    }
                }
            }
            res.MappingValid = (res.Complaints.Count() == 0);
            return res;
        }

        // internally, CrissCross uses yyyy-mm-dd to pass dates from client to server
        // sometimes also needs to parse dates from executionlog that are in regional format
        // hence this tries specific regional format then general format.
        // Also FlexiEnGbUs is a magic option to try both Gb and Us in a safe way
        public DateParseResult ParseDateStringVariousWays(string dateString, string cultureCode)
        {
            var ret = new DateParseResult();
            
            DateTime parseResult;
            // first try specific culture
            // specific flexible option for gb/us dates
            if (cultureCode.ToLower().Equals("flexi-gb-us"))
            {
                if (dateString.EndsWith("12:00:00 AM"))
                    cultureCode = "EN-US";
                else if (dateString.EndsWith("00:00:00"))
                    cultureCode = "EN-GB";
                else
                    cultureCode = null;
            }
            if (cultureCode != null)
            {
                IFormatProvider culture = new CultureInfo(cultureCode, true);
                // parse using a culture code
                if (DateTime.TryParse(dateString, culture, DateTimeStyles.None, out parseResult))
                {
                    ret.CouldParse = true;
                    ret.DateTime = parseResult;
                    return ret;
                }
            }
            
            // then try generally
            if (DateTime.TryParse(dateString, out parseResult))
            {
                ret.CouldParse = true;
                ret.DateTime = parseResult;
                return ret;
            }
            ret.CouldParse = false;
            return ret;

        }

        private string GetCultureForDateParsing()
        {
            return ConfigurationManager.AppSettings["crisscross.CultureForDateParsing"];
        }


        public class DateParseResult
        {
            public bool CouldParse { get; set; }
            public DateTime DateTime { get; set; }

        }

        public class ParameterMapResult
        {
            public bool MappingValid { get; set; }
            public List<string> Complaints { get; set; }

        }

    }
}
