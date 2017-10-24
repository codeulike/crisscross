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
using CrissCrossLib.ReportWebService;

namespace CrissCrossLib.Hierarchical
{
    public class CrcReportFolderFactory
    {
        public CrcReportFolder Create(ReportingService2005Soap rService)
        {
            return Create(rService, @"/");
        }

        public CrcReportFolder Create(ReportingService2005Soap rService, string path)
        {
            var ret = new CrcReportFolder();
            ret.Path = path;
            ret.FolderName = CrcReportDefinition.ReportNameFromPath(path);

            var lcRequest = new ListChildrenRequest(path, false);
            
            var lcResponse = rService.ListChildren(lcRequest);
            foreach (CatalogItem itemLoop in lcResponse.CatalogItems)
            {
                if (itemLoop.Type == ItemTypeEnum.Folder)
                {
                    var sf = Create(rService, itemLoop.Path);
                    if (sf.Reports.Count() > 0 || sf.SubFolders.Count() > 0)
                        ret.SubFolders.Add(sf);
                }
                else if (itemLoop.Type == ItemTypeEnum.Report)
                {
                    if (!itemLoop.Hidden)
                    {
                        var repItem = new CrcReportItem();
                        repItem.ReportPath = itemLoop.Path;
                        repItem.DisplayName = itemLoop.Name;
                        repItem.Description = itemLoop.Description;
                        ret.Reports.Add(repItem);
                    }
                }

            }
            return ret;
        }

     
    }
}
