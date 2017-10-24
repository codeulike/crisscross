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
    public class CrcParameterDefinition : IDeepCloneable<CrcParameterDefinition>
    {

        public CrcParameterDefinition()
        {
            ValidValues = new List<CrcValidValue>();
            EmptyEquivalentValues = new List<string>();
            DependantParameterNames = new List<string>();
            DependantParameterIds = new List<string>();
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string id { get; set;}
        

        public CrcParameterType ParameterType { get; set; }
        
        public List<CrcValidValue> ValidValues { get; set; }

        public List<string> EmptyEquivalentValues { get; set; }

        public CrcParameterChoice ParameterChoice { get; set; }

        public bool Hidden { get; set; }

        public bool AlwaysShow { get; set; }

        public bool AllowListSearch { get; set; }

        public bool AllowNull { get; set; }

        public bool AllowBlank { get; set; }

        public bool RequiredFromUser { get; set; }

        public List<string> DependantParameterNames { get; set; }

        public List<string> DependantParameterIds { get; set; }

        public bool IsEmptyEquivalent
        {
            get
            {
                if (this.ParameterChoice == null)
                    return true;
                bool emptyEquivFound = false;
                foreach (string e in this.EmptyEquivalentValues)
                    if (this.ParameterChoice.Values.Contains(e))
                        emptyEquivFound = true;
                return emptyEquivFound;
            }
        }

        
        public CrcParameterDefinition DeepClone()
        {
            var clone = new CrcParameterDefinition()
            {
                Name = this.Name,
                id = this.id,
                DisplayName = this.DisplayName,
                ParameterType = this.ParameterType,
                AllowListSearch = this.AllowListSearch,
                AlwaysShow = this.AlwaysShow,
                Hidden = this.Hidden,
                AllowBlank = this.AllowBlank,
                AllowNull = this.AllowNull,
                RequiredFromUser = this.RequiredFromUser
            };

            if (this.ParameterChoice != null)
                clone.ParameterChoice = this.ParameterChoice.DeepClone();
            
            foreach (var valLoop in this.ValidValues)
                clone.ValidValues.Add(valLoop.DeepClone());
            foreach (var sLoop in this.EmptyEquivalentValues)
                clone.EmptyEquivalentValues.Add(sLoop);
            foreach (var sLoop in this.DependantParameterIds)
                clone.DependantParameterIds.Add(sLoop);
            foreach (var sLoop in this.DependantParameterNames)
                clone.DependantParameterNames.Add(sLoop);
            return clone;
        }

        
    }

    public class CrcValidValue : IDeepCloneable<CrcValidValue>
    {
        public string Value { get; set; }
        public string Label { get; set; }

        public CrcValidValue DeepClone()
        {
            return new CrcValidValue() { Label = this.Label, Value = this.Value };
        }

    }

    public enum CrcParameterType
    {
        Text = 1,
        Date = 2,
        Select = 3,
        MultiSelect =4,
        Boolean = 5
    }
}
