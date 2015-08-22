using Microsoft.SharePoint;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SPMin.Controls
{
    public class AssetRegistrationControl : WebControl
    {
        [Category("Settings")]
        [Bindable(true)]
        [DefaultValue("")]
        [Localizable(true)]
        public string FilePath
        {
            get { return Convert.ToString(ViewState["FilePath"]) ?? ""; }
            set { ViewState["FilePath"] = value; }
        }

        private EnvironmentMode? _environmentMode;
        private EnvironmentMode EnvironmentMode
        {
            get
            {
                if (!_environmentMode.HasValue)
                {
                    // Emulate production environment while in development environment
                    if (HttpContext.Current.Request.QueryString["spmin"] == "production")
                        _environmentMode = EnvironmentMode.Production;
                    else
                        _environmentMode = new Environment(SPContext.Current.Site).Mode;
                }

                return _environmentMode.Value;
            }
        }

        protected string GetFinalPath()
        {
            string path = FilePath;

            if (EnvironmentMode == EnvironmentMode.Production)
            {
                string fileName = Path.GetFileName(FilePath);
                var fileNameParser = new FileNameParser(fileName);

                if (fileNameParser.ShouldBeMinified)
                {
                    string directoryPath = FileUtilities.GetDirectoryPathFromFilePath(path);
                    path = String.Format("{0}/{1}", directoryPath, fileNameParser.MinifiedVersionFileName);
                }
            }

            path = String.Format("{0}/Style Library/{1}", SPContext.Current.Site.RootWeb.ServerRelativeUrl, path);
            path = FileUtilities.RemoveDuplicatedSlashesFromPath(path);

            return path;
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            var html = new StringBuilder();

            if (EnvironmentMode == EnvironmentMode.Development)
                GenerateIncludeScriptTags(html);

            string finalFilePath = GetFinalPath();
            html.AppendLine(GenerateHtml(finalFilePath));

            output.Write(html.ToString());
        }

        protected void GenerateIncludeScriptTags(StringBuilder html)
        {
            SPWeb web = SPContext.Current.Site.RootWeb;

            string filePath = String.Format("{0}/Style Library/{1}", web.ServerRelativeUrl, FilePath);
            filePath = FileUtilities.RemoveDuplicatedSlashesFromPath(filePath);

            try
            {
                SPFile file = web.GetFile(filePath);
                var reader = new FileReader(file);
                string fileDirectory = FileUtilities.GetDirectoryPathFromFilePath(filePath);
             
                foreach (string includedFile in reader.IncludedFiles)
                    html.AppendLine(GenerateHtml(fileDirectory + "/" + includedFile));
            }
            catch (SPException)
            {
                html.AppendLine("<!-- SPMin: " + HttpUtility.HtmlEncode(filePath) + " not found -->");
            }
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            writer.Write("");
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
            writer.Write("");
        }

        public virtual string GenerateHtml(string path)
        {
            throw new NotImplementedException();
        }
    }
}
