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

        // In ReportService2005 endpoint, Catalog ItemTypes were an enum
        // In ReportService2010 endpoint, ItemTypes are represented by strings
        // the ws method ListItemTypes() is supposed to return these types
        // but in practice they appear to be fixed anyway.
        // see https://msdn.microsoft.com/en-us/library/reportservice2010.reportingservice2010.listitemtypes.aspx
        // Hence this static class operates somwhat like an Enum,
        // but with String values
        public static class ReportServiceItemTypes
        {
            public static readonly String Component = "Component"; // a report part
            public static readonly String DataSource = "DataSource";
            public static readonly String Folder = "Folder";
            public static readonly String Model = "Model"; // model for report builder
            public static readonly String LinkedReport = "LinkedReport";
            public static readonly String Report = "Report";
            public static readonly String Resource = "Resource";
            public static readonly String DataSet = "DataSet";
            public static readonly String Site = "Site"; // a sharepoint site
            public static readonly String Unknown = "Unknown"; // An item not associated with any known type.
            

        }

        public CrcReportFolder Create(ReportingService2010Soap rService)
        {
            return Create(rService, @"/");
        }

        public CrcReportFolder Create(ReportingService2010Soap rService, string path)
        {
            var ret = new CrcReportFolder();
            ret.Path = path;
            ret.FolderName = CrcReportDefinition.ReportNameFromPath(path);

            
            var lcRequest = new ListChildrenRequest(new TrustedUserHeader(), path, false);
            
            
            var lcResponse = rService.ListChildren(lcRequest);
            foreach (CatalogItem itemLoop in lcResponse.CatalogItems)
            {
                
                if (itemLoop.TypeName != null && itemLoop.TypeName.Equals(ReportServiceItemTypes.Folder))
                {
                    var sf = Create(rService, itemLoop.Path);
                    if (sf.Reports.Count() > 0 || sf.SubFolders.Count() > 0)
                        ret.SubFolders.Add(sf);
                }
                else if (itemLoop.TypeName != null && itemLoop.TypeName.Equals(ReportServiceItemTypes.Report))
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
