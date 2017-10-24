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
using System.Xml.Serialization;

namespace CrissCrossLib.Configuration
{
    [Serializable]
    public class CrcReportConfig
    {
        public CrcReportConfig()
        {
            CrcParamConfigs = new List<CrcParamConfig>();
        }

        public string Path { get; set; }
        public string ReportHint { get; set; }
        public bool IsFeatured { get; set; }

        public List<CrcParamConfig> CrcParamConfigs { get; set; }

        

        [Serializable]
        public class CrcParamConfig
        {
            public CrcParamConfig()
            {
                DependantParams = new List<string>();
                EmptyEquivalentValues = new List<string>();
            }

            public string ParamName { get; set; }
            public bool ShowByDefault { get; set; }

            [XmlArray]
            [XmlArrayItem(ElementName = "DependantParam")]
            public List<string> DependantParams { get; set; }

            [XmlArray]
            [XmlArrayItem(ElementName = "EmptyEquivalentValue")]
            public List<string> EmptyEquivalentValues { get; set; }

        }

        public List<string> GetParamsToShowByDefault()
        {
            return CrcParamConfigs.Where(p => p.ShowByDefault).Select(p => p.ParamName).ToList<string>();
        }

        public bool DependantParamsSpecified
        {
            get
            {
                return CrcParamConfigs.Exists(p => p.DependantParams != null && p.DependantParams.Count() > 0);
            }
        }
    }
}
