using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace CrissCross.Help
{
    public partial class HelpIntro : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                uxAppTitle.Text = (Page.Master as Main).AppTitle;
                uxAppTitle2.Text = (Page.Master as Main).AppTitle;
                string reportViewerEndpoint = ConfigurationManager.AppSettings["crisscross.ReportServerRootUrl"];
                string reportManagerUrl = reportViewerEndpoint.Replace("reportserver", "reports");

                uxReportManagerLink.NavigateUrl = reportManagerUrl;
                uxReportManagerLink.Text = reportManagerUrl;


            }
        }
    }
}