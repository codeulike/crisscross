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
    public class CrcReportHistoryFetcherDefault : IHistoryFetcher
    {
        public List<string> GetUsersFavourites(string username)
        {
            string sql = SqlPatternUsersFavourites();
            DataTable res = new DataTable();
            using (SqlConnection conn = new SqlConnection(GetReportServerConnectionString()))
            {
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.Parameters.Add("@UserName", SqlDbType.NVarChar, 520).Value = username;
                SqlDataAdapter adapt = new SqlDataAdapter(comm);
                adapt.Fill(res);
            }
            List<string> topReportPaths = res.AsEnumerable().Select(r => r.Field<string>("ReportPath")).ToList();
            return topReportPaths;
        }

        protected virtual string SqlPatternUsersFavourites()
        {
            return "select top 5 ReportPath, COUNT(*) as Runs from ExecutionLog2 with (nolock) "
            + " where Source = 'Live' and UserName = @UserName "
            + " group by ReportPath order by Runs desc";
        }

        public List<string> GetGlobalFavourites()
        {
            string sql = SqlPatternGlobalFavourites();
            DataTable res = new DataTable();
            using (SqlConnection conn = new SqlConnection(GetReportServerConnectionString()))
            {
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataAdapter adapt = new SqlDataAdapter(comm);
                adapt.Fill(res);
            }
            List<string> topReportPaths = res.AsEnumerable().Select(r => r.Field<string>("ReportPath")).ToList();
            return topReportPaths;
        }

        protected virtual string SqlPatternGlobalFavourites()
        {
            return "select top 5 ReportPath, COUNT(*) as Runs from ExecutionLog2 with (nolock) "
            + " where Source = 'Live'"
            + " group by ReportPath order by Runs desc";
        }

        public List<string> GetGlobalFavouriteFolders()
        {
            string sql = SqlPatternGlobalFavouriteFolders();
            DataTable res = new DataTable();
            using (SqlConnection conn = new SqlConnection(GetReportServerConnectionString()))
            {
                SqlCommand comm = new SqlCommand(sql, conn);
                SqlDataAdapter adapt = new SqlDataAdapter(comm);
                adapt.Fill(res);
            }
            List<string> topFolders = res.AsEnumerable().Select(r => r.Field<string>("Folder")).ToList();
            return topFolders;
        }

        protected virtual string SqlPatternGlobalFavouriteFolders()
        {
            return "select top 5 LEFT(ReportPath, len(ReportPath) - CHARINDEX('/', Reverse(ReportPath)) + 1) as Folder, COUNT(*) as Runs "
                    + "from ExecutionLog2 with (nolock) "
                    + "where Source = 'Live' "
                    + "and ReportPath <> 'Unknown' "
                    + "group by LEFT(ReportPath, len(ReportPath) - CHARINDEX('/', Reverse(ReportPath)) + 1) "
                    + "order by runs desc";
        }

        public List<string> GetUsersFavouriteFolders(string username)
        {
            string sql = SqlPatternUsersFavouriteFolders();
            DataTable res = new DataTable();
            using (SqlConnection conn = new SqlConnection(GetReportServerConnectionString()))
            {
                SqlCommand comm = new SqlCommand(sql, conn);
                comm.Parameters.Add("@UserName", SqlDbType.NVarChar, 520).Value = username;
                SqlDataAdapter adapt = new SqlDataAdapter(comm);
                adapt.Fill(res);
            }
            List<string> topFolders = res.AsEnumerable().Select(r => r.Field<string>("Folder")).ToList();
            // if less than 5 top up from global most used
            if (topFolders.Count() < 5)
            {
                int shortfall = 5 - topFolders.Count();
                List<string> globalFolders = GetGlobalFavouriteFolders();
                for (int i = 0; i < shortfall; i++)
                {
                    if (i < globalFolders.Count())
                        topFolders.Add(globalFolders[i]);
                }

            }
            return topFolders;
        }

        protected virtual string SqlPatternUsersFavouriteFolders()
        {
            return "select top 5 LEFT(ReportPath, len(ReportPath) - CHARINDEX('/', Reverse(ReportPath)) + 1) as Folder, COUNT(*) as Runs "
                    + "from ExecutionLog2 with (nolock) "
                    + "where Source = 'Live' and UserName = @UserName "
                    + "and ReportPath <> 'Unknown' "
                    + "group by LEFT(ReportPath, len(ReportPath) - CHARINDEX('/', Reverse(ReportPath)) + 1) "
                    + "order by runs desc";
        }

        public List<CrcReportHistory> GetUsersRecentRuns(string username, int max)
        {
            string sql = SqlPatternUsersRecentRuns();
            DataTable res = new DataTable();
            using (SqlConnection conn = new SqlConnection(GetReportServerConnectionString()))
            {
                SqlCommand comm = new SqlCommand(String.Format(sql,max), conn);
                
                comm.Parameters.Add("@UserName", SqlDbType.NVarChar, 520).Value = username;
                SqlDataAdapter adapt = new SqlDataAdapter(comm);
                adapt.Fill(res);
            }
            var histList = new List<CrcReportHistory>();
            foreach (DataRow rowLoop in res.Rows)
            {
                var h = new CrcReportHistory()
                {
                    ReportName = CrcReportDefinition.ReportNameFromPath(rowLoop["ReportPath"].ToString()),
                    ReportPath = rowLoop["ReportPath"].ToString(),
                    UserName = rowLoop["UserName"].ToString(),
                    Parameters = rowLoop["Parameters"].ToString(),
                    TimeStart = (DateTime)rowLoop["TimeStart"],
                    RunDuration = (int)rowLoop["TimeDataRetrieval"],
                    RowCount = (long)rowLoop["RowCount"]
                };
                histList.Add(h);
            }
            return histList;

        }

        protected virtual string SqlPatternUsersRecentRuns()
        {
            return "select top {0} * from ExecutionLog2 with (nolock) "
            + " where Source = 'Live' and UserName = @UserName "
            + " order by TimeStart desc";
        }

        private string GetReportServerConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ReportServerDb"].ConnectionString;
        }
    }
}
