using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrissCrossLib;

namespace CrissCross.Admin
{
    public partial class UserHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void uxGetHistory_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
            string forUser = uxUsername.Text;
            var crcr = new CrissCrossServices();

            uxRecentListView.DataSource = crcr.GetUsersRecentRuns(forUser, 200);
            uxRecentListView.DataBind();
        }
    }
}
