using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using CrissCrossLib;
using CrissCrossLib.ReportWebService;
using CrissCrossLib.Hierarchical;
using log4net;

namespace CrissCross
{
    public partial class HierarchicalCatalog : System.Web.UI.UserControl
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(HierarchicalCatalog));

        protected void Page_Load(object sender, EventArgs e)
        {
            ShowReportLists(this.InitialFolder, this.FullUsername);
        }

        public string FullUsername
        {
            get{ return uxHiddenFullUsername.Text;}
            set{ uxHiddenFullUsername.Text = value;}
        }

        public string InitialFolder
        {
            get { return uxHiddenInitialFolder.Text; }
            set { uxHiddenInitialFolder.Text = value; }
        }

        private void ShowReportLists(string showFolder, string username)
        {
            logger.DebugFormat("Showing hierarchical catalog for {0} initial folder {1}",
                username, showFolder);
            var crcr = new CrissCrossServices();


            var repFolderTree = crcr.GetAllReportsHierarchical(username);
            repFolderTree.FolderName = "All Reports";
            uxAllCatalogHierarchical.Text = string.Format("<div class=\"hierarchicalCatalog\">{0}</div>", HierarchicalCatalogView(repFolderTree, 0, showFolder));


        }

        private string HierarchicalCatalogView(CrcReportFolder rootFolder, int level, string showFolder)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class=\"folderBox\">");
            string scrollTo = "";
            if (PathMatch(showFolder, rootFolder.Path))
                scrollTo = " scrollToFolder";
            sb.AppendFormat("<div class=\"folderName{1}\">{0}</div>", rootFolder.FolderName, scrollTo);
            string show = "none";
            if (level == 0 || PathContains(showFolder, rootFolder.Path))
                show = "block";

            sb.AppendFormat("<div class=\"folderChildren\" style=\"display:{0}\">", show);

            foreach (CrcReportFolder subFolderLoop in rootFolder.SubFolders)
                sb.Append(HierarchicalCatalogView(subFolderLoop, level + 1, showFolder));

            foreach (CrcReportItem itemLoop in rootFolder.Reports)
            {

                sb.Append("<div class=\"reportRow\">");
                sb.AppendFormat("<a class=\"reportLink vanillaHover\" href=\"Report.aspx?path={0}\" >{1}</a>",
                    Server.UrlEncode(itemLoop.ReportPath), itemLoop.DisplayName);
                if (!string.IsNullOrEmpty(itemLoop.ShortDescription))
                    sb.AppendFormat("<div class=\"reportInfo\">{0}</div>", itemLoop.ShortDescription);
                sb.Append("<div class=\"clear\"></div></div>");

            }

            sb.Append("</div></div>");
            return sb.ToString();

        }

        private bool PathContains(string path, string partialPath)
        {
            if (path == null)
                return false;

            return path.StartsWith(partialPath);
        }

        private bool PathMatch(string path, string otherPath)
        {
            if (path == null)
                return false;

            return path.TrimEnd("/".ToCharArray()).Equals(otherPath.TrimEnd("/".ToCharArray()));
        }
    }
}