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

namespace CrissCrossLib
{
    public class CrcReportDefinition : IDeepCloneable<CrcReportDefinition>
    {
        public CrcReportDefinition()
        {
            ParameterDefinitions = new List<CrcParameterDefinition>();
        }

        public string ReportPath { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string ReportHint { get; set; }

        public List<CrcParameterDefinition> ParameterDefinitions { get; set; }


        public bool HasDependantParameters
        {
            get
            {
                return ParameterDefinitions.Exists(p => p.DependantParameterNames.Count > 0);
            }
        }

        

        public string GetReportFolder()
        {
            string folder = this.ReportPath;
            if (folder.LastIndexOf("/") > -1 )
                folder = folder.Substring(0, folder.LastIndexOf("/")+1);
            else
                folder = "/";
            return folder;
        }


        /// <summary>
        /// returns parameter name/displayname dictionary
        /// contains only parameters that user can set. (ie not the hidden ones)
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetAvailableParameterNames()
        {
            return this.ParameterDefinitions.Where(p => p.Hidden == false).ToDictionary(p => p.Name, p => p.DisplayName);
        }

        /// <summary>
        /// Returns parameters that:
        /// - have chosen values
        /// - are 'alwaysshow'
        /// </summary>
        public List<CrcParameterDefinition> GetMinimumParametersToDisplay()
        {
            return this.ParameterDefinitions.Where(p => !p.Hidden && (!p.IsEmptyEquivalent || p.AlwaysShow || p.RequiredFromUser)).ToList();
        }

        public CrcParameterChoiceMapper.ParameterMapResult MapParameterChoices(CrcParameterChoiceCollection paramChoices)
        {
            // dry run on a clone first to check for errors
            var clone = this.DeepClone();
            var mapper = new CrcParameterChoiceMapper();
            var verify = mapper.MapParameterChoices(clone, paramChoices);
            if (!verify.MappingValid)
                return verify;

            // if still here then there are no errors - map to real object
            var results = mapper.MapParameterChoices(this, paramChoices);
            return results;

        }

        
        public CrcReportDefinition DeepClone()
        {
            var clone = new CrcReportDefinition()
            {
                DisplayName = this.DisplayName,
                ReportPath = this.ReportPath,
                Description = this.Description,
                ReportHint = this.ReportHint
            };
            foreach (var paramLoop in this.ParameterDefinitions)
                clone.ParameterDefinitions.Add(paramLoop.DeepClone());

            return clone;
        }

        public static string ReportNameFromPath(string path)
        {
            string name = path;
            if (name.LastIndexOf("/") > -1 && name.LastIndexOf("/") < name.Length -1)
                    name = name.Substring(name.LastIndexOf("/")+1);
            else if (name == "/")
                name = "";
            return name;
        }

    }
}
