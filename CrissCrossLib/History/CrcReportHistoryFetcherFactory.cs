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
using System.Configuration;

namespace CrissCrossLib.History
{
    public class CrcReportHistoryFetcherFactory
    {
        public virtual IHistoryFetcher Create()
        {
            // TODO: at the moment this just returns default but eventually will return different history fetchers based on config
            // e.g different ones for
            // -SSRS 2005 
            // -SSRS 2008
            // -log data extracted somewhere else as recommended by http://technet.microsoft.com/en-us/library/ms155836(v=sql.100).aspx
            // -log data extracted by Scrubs http://scrubs.codeplex.com/
            IHistoryFetcher ret = null;
            switch (this.ReportHistoryFormat)
            {
                case "2008":
                    ret = new CrcReportHistoryFetcherDefault();
                    break;

                case "2005":
                    ret = new CrcReportHistoryFetcher2005();
                    break;

                default:
                    throw new ApplicationException(string.Format("Unknown crisscross.ReportHistoryFormat of {0} specified",
                        this.ReportHistoryFormat));
                    break;

            }
            return ret;
        }

        private string ReportHistoryFormat
        {
            get
            {
                if (ConfigurationManager.AppSettings["crisscross.ReportHistoryFormat"] == null)
                    // return default
                    return "2008";
                return (ConfigurationManager.AppSettings["crisscross.ReportHistoryFormat"]);
            }
        }

    }
}
