using Microsoft.SharePoint;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
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

            return filePath;
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            Environment environment = EnvironmentDetector.Detect(SPContext.Current.Site);
            output.Write(GenerateHtml(environment));
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            writer.Write("");
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
            writer.Write("");
        }

        public virtual string GenerateHtml(Environment environment)
        {
            throw new NotImplementedException();
        }
    }
}
