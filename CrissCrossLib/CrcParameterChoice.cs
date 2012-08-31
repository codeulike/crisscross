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

namespace CrissCrossLib
{
    public class CrcParameterChoice: IDeepCloneable<CrcParameterChoice>
    {
        public CrcParameterChoice()
        {
            Values = new List<string>();
        }

        public CrcParameterChoice(string name)
            : this()
        {
            Name = name;
        }

        public string Name { get; set; }
        public List<string> Values { get; set; }

        public string SingleValue
        {
            get {
                if (Values.Count() > 0)
                    return Values[0];
                else
                    return null;
            }
            set
            {
                if (Values.Count() > 1)
                    throw new ApplicationException("Cannot use set SingleValue when more than 1 value in collection");
                if (Values.Count() == 0)
                    Values.Add(value);
                else
                    Values[0] = value;

            }
        }




        

        public CrcParameterChoice DeepClone()
        {
            var clone = new CrcParameterChoice
            {
                Name = this.Name,
            };

            foreach (var valLoop in this.Values)
                clone.Values.Add(valLoop);
            return clone;
        }

        
    }
}
