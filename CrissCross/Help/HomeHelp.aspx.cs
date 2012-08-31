using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace CrissCross.Help
{
    public partial class HomeHelp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                

                uxFeaturedReportsTitle.Text = FeaturedReportsTitle;
            }
        }

        public string FeaturedReportsTitle
        {
            get
            {
                if (ConfigurationManager.AppSettings["crisscross.FeaturedReportsTitle"] == null)
                    return "Featured Reports";
                return ConfigurationManager.AppSettings["crisscross.FeaturedReportsTitle"];
            }
        }
    }
}
