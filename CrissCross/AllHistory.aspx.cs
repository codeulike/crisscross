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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrissCrossLib;

namespace CrissCross
{
    public partial class AllHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {

                string username = User.Identity.Name.ToString();
                uxHiddenFullUsername.Text = username;
                if (username.IndexOf(@"\") > -1)
                    username = username.Split(@"\".ToCharArray()).Last();
                uxUserName.Text = username;

                ShowReportLists();
            }
        }

        private void ShowReportLists()
        {
            var crcr = new CrissCrossServices();
            
            uxRecentListView.DataSource = crcr.GetUsersRecentRuns(uxHiddenFullUsername.Text, 200);
            uxRecentListView.DataBind();




        }
    }
}
