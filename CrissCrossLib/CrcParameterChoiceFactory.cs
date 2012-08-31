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
using System.Web;

namespace CrissCrossLib
{
    public class CrcParameterChoiceFactory
    {
        public CrcParameterChoiceCollection Create(string paramString)
        {
            var pcCol = new CrcParameterChoiceCollection();
            string[] clauses = paramString.Split("&".ToCharArray());
            if (clauses.Count() == 1 && string.IsNullOrEmpty(clauses[0]))
                return pcCol;
            foreach (string clauseLoop in clauses)
            {
                int isnullPos = clauseLoop.IndexOf(":isnull");
                int equalsPos = clauseLoop.IndexOf("=");
                string pname = null;
                string pval = null;
                if (isnullPos > 0)
                {
                    pname = clauseLoop.Substring(0, isnullPos);
                    string boolString = clauseLoop.Substring(isnullPos + 8);
                    bool isNullValue = bool.Parse( boolString);
                    if (!isNullValue)
                        throw new ApplicationException(String.Format("not sure how to interpret false in clause {0}", clauseLoop));
                    else
                        pval = null;
                }
                else if (equalsPos > 0)
                {
                    pname = clauseLoop.Substring(0, equalsPos);
                    pval = HttpUtility.UrlDecode(clauseLoop.Substring(equalsPos + 1));
                }
                else
                    throw new ApplicationException(String.Format("Could not parse clause {0}", clauseLoop));

                // we now have a pname and pval
                var pcExists = pcCol.ParameterChoiceList.FirstOrDefault(p => p.Name == pname);
                if (pcExists == null)
                {
                    var newPc = new CrcParameterChoice(pname);
                    pcCol.ParameterChoiceList.Add(newPc);
                    pcExists = newPc;
                }
                pcExists.Values.Add(pval);
            }
            return pcCol;
        }
    }
}
