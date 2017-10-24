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
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace CrissCrossLib.History
{
    /// <summary>
    /// SSRS 2005 doesn't have the ExecutionLog2 view so this version 
    /// overrides CrcReportHistoryFetcherDefault to use the ExecutionLog view that is present in SSRS 2005
    /// Thanks to Teleute for figuring out the sql
    /// </summary>
    public class CrcReportHistoryFetcher2005 : CrcReportHistoryFetcherDefault
    {
        
        protected override string SqlPatternUsersFavourites()
        {
            return "SELECT TOP (5) c.Path AS ReportPath, COUNT(*) AS Runs FROM ExecutionLog el WITH (nolock) "
            + " LEFT OUTER JOIN Catalog AS c WITH (nolock) ON el.ReportID = c.ItemID "
            + " WHERE el.Source = 1 AND el.UserName = @UserName AND c.Path IS NOT NULL "
            + " GROUP BY c.Path ORDER BY Runs DESC"; 
        }

       

        protected override string SqlPatternGlobalFavourites()
        {
            return "SELECT TOP (5) c.Path AS ReportPath, COUNT(*) AS Runs FROM ExecutionLog el WITH (nolock) "
            + " LEFT OUTER JOIN Catalog AS c WITH (nolock) ON el.ReportID = c.ItemID "
            + " WHERE el.Source = 1 AND c.Path IS NOT NULL "
            + " GROUP BY c.Path ORDER BY Runs DESC";
        }



        protected override string SqlPatternGlobalFavouriteFolders()
        {
            return "select top 5 LEFT(c.Path, len(c.Path) - CHARINDEX('/', Reverse(c.Path)) + 1) as Folder, COUNT(*) as Runs "
                    + "from ExecutionLog el with (nolock) "
                    + " LEFT OUTER JOIN Catalog AS c WITH (nolock) ON el.ReportID = c.ItemID "
                    + "where el.Source = 1 "
                    + "and c.Path <> 'Unknown' and c.Path is not null "
                    + "group by LEFT(c.Path, len(c.Path) - CHARINDEX('/', Reverse(c.Path)) + 1) "
                    + "order by runs desc";
        }



        protected override string SqlPatternUsersFavouriteFolders()
        {
            return "select top 5 LEFT(c.Path, len(c.Path) - CHARINDEX('/', Reverse(c.Path)) + 1) as Folder, COUNT(*) as Runs "
                    + "from ExecutionLog el with (nolock) "
                    + " LEFT OUTER JOIN Catalog AS c WITH (nolock) ON el.ReportID = c.ItemID "
                    + "where el.Source = 1 and el.UserName = @UserName "
                    + "and c.Path <> 'Unknown' and c.Path is not null "
                    + "group by LEFT(c.Path, len(c.Path) - CHARINDEX('/', Reverse(c.Path)) + 1) "
                    + "order by runs desc";
        }


        protected override string SqlPatternUsersRecentRuns()
        {
            return "SELECT TOP {0} el.*, c.Path AS ReportPath FROM ExecutionLog el WITH (nolock) "
            + " LEFT OUTER JOIN Catalog AS c WITH (nolock) ON el.ReportID = c.ItemID "
            + " WHERE el.Source = 1 AND el.UserName = @UserName AND c.Path IS NOT NULL "
            + " ORDER BY el.TimeStart DESC";
        }

    }
}
