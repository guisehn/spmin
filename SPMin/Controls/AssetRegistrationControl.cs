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

        protected string GetFinalPath(Environment environment)
        {
            string filePath = FilePath;

            if (environment == Environment.Production)
            {
                string fileName = Path.GetFileName(FilePath);
                var fileNameParser = new FileNameParser(fileName);

                if (fileNameParser.ShouldBeMinified)
                    filePath = Regex.Replace(filePath, "([^/]+)$", fileNameParser.MinifiedVersionFileName);
            }

            return String.Format("{0}/Style Library/{1}", SPContext.Current.Web.ServerRelativeUrl, filePath);
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            Environment environment = EnvironmentDetector.Detect(SPContext.Current.Site);
            var html = new StringBuilder();

            if (environment == Environment.Development)
            {
                SPWeb web = SPContext.Current.Site.RootWeb;
                string fileUrl = String.Format("{0}/Style Library/{1}", web.ServerRelativeUrl, FilePath);
                string[] fileParts = fileUrl.Split('/');
                string fileDirectory = String.Join("/", fileParts.Take(fileParts.Length - 1).ToArray());

                try
                {
                    SPFile file = web.GetFile(fileUrl);
                    var reader = new FileReader(file);

                    foreach (string includedFile in reader.IncludedFiles)
                        html.AppendLine(GenerateHtml(fileDirectory + "/" + includedFile));
                }
                catch (SPException)
                {
                    html.AppendLine("<!-- SPMin: " + HttpUtility.HtmlEncode(fileUrl) + " not found -->");
                }
            }

            string finalFilePath = GetFinalPath(environment);
            html.AppendLine(GenerateHtml(finalFilePath));

            output.Write(html.ToString());
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
