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
    public abstract class AssetRegistrationControl : WebControl
    {
        private EnvironmentMode? _environmentMode = null;
        private bool _includeOnce = true;
        private StringBuilder html = new StringBuilder();

        [Category("Settings")]
        public bool IncludeOnce
        {
            get { return _includeOnce; }
            set { _includeOnce = value; }
        }

        [Category("Settings")]
        public virtual bool AddToHead { get; set; }

        [Category("Settings")]
        public string FilePath { get; set; }

        /// <summary>
        /// Gets the current environment mode
        /// </summary>
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

        /// <summary>
        /// The key used to verificate if the selected asset has been already included in the current request
        /// </summary>
        private string InclusionVerificationKey
        {
            get
            {
                return "SPMinIncluded_" + FilePath;
            }
        }

        /// <summary>
        /// Checks if the selected asset has been already included in the current request
        /// </summary>
        private bool AlreadyIncludedInCurrentRequest
        {
            get
            {
                return HttpContext.Current.Items[InclusionVerificationKey] != null;
            }
        }

        /// <summary>
        /// Marks the selected asset as included for the current request.
        /// </summary>
        protected void MarkAssetAsIncluded()
        {
            if (!AlreadyIncludedInCurrentRequest)
                HttpContext.Current.Items[InclusionVerificationKey] = true;
        }

        /// <summary>
        /// Checks if the control should be rendered
        /// </summary>
        protected bool ShouldRender
        {
            get
            {
                return !IncludeOnce || !AlreadyIncludedInCurrentRequest;
            }
        }

        /// <summary>
        /// Gets the final path for the specified asset based on the environment mode
        /// </summary>
        /// <returns>Final path</returns>
        protected string GetFinalPath()
        {
            string path = FilePath;

            if (EnvironmentMode == EnvironmentMode.Production)
            {
                var fileNameParser = new FileNameParser(path);

                if (fileNameParser.ShouldBeMinified)
                {
                    var fileHashDictionary = new FileHashDictionary(SPContext.Current.Site);
                    var fileHash = fileHashDictionary[path];
                    var minifiedFileName = fileNameParser.GenerateMinifiedVersionFileName(fileHash);

                    path = String.Format("{0}/{1}", Constants.SPMinFolderName, minifiedFileName);
                }
            }

            path = String.Format("{0}/Style Library/{1}", SPContext.Current.Site.RootWeb.ServerRelativeUrl, path);
            path = FileUtilities.RemoveDuplicatedSlashesFromPath(path);

            return path;
        }

        /// <summary>
        /// Generates the HTML for the separated inclusion tags
        /// </summary>
        protected void GenerateSeparatedInclusionTags()
        {
            SPWeb web = SPContext.Current.Site.RootWeb;

            string filePath = String.Format("{0}/Style Library/{1}", web.ServerRelativeUrl, FilePath);
            filePath = FileUtilities.RemoveDuplicatedSlashesFromPath(filePath);

            try
            {
                SPFile file = web.GetFile(filePath);
                var reader = new FileContentParser(file);
                string fileDirectory = FileUtilities.GetDirectoryPathFromFilePath(filePath);

                foreach (string includedFile in reader.IncludedFiles)
                    html.AppendLine(GenerateHtml(fileDirectory + "/" + includedFile));
            }
            catch (SPException)
            {
                html.AppendLine("<!-- SPMin: " + HttpUtility.HtmlEncode(filePath) + " not found -->");
            }
        }

        /// <summary>
        /// Executes before the control is rendered
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            if (!ShouldRender)
                return;

            // Generates separated inclusion tags when in development mode
            if (EnvironmentMode == EnvironmentMode.Development)
                GenerateSeparatedInclusionTags();

            // Generates the asset inclusion tag based on environment mode
            string finalFilePath = GetFinalPath();
            html.AppendLine(GenerateHtml(finalFilePath));

            if (AddToHead)
                RenderInHead();
        }

        /// <summary>
        /// Renders in the place where the control was included
        /// </summary>
        /// <param name="output">Variable to write the output</param>
        protected override void RenderContents(HtmlTextWriter output)
        {
            if (!ShouldRender)
                return;

            MarkAssetAsIncluded();

            if (AddToHead)
                output.Write("<!-- head: {0} -->", HttpUtility.HtmlEncode(FilePath));
            else
                output.Write(html.ToString());
        }

        /// <summary>
        /// Renders the inclusion tag inside the additional page head placeholder
        /// </summary>
        protected void RenderInHead()
        {
            Control control = Page.Controls[0].FindControl("PlaceHolderAdditionalPageHead");

            if (control == null)
                throw new SPMinException("PlaceHolderAdditionalPageHead not found");

            control.Controls.Add(new LiteralControl { Text = html.ToString() });
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            // This method is overridden to remove WebControl's default <span> container
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
            // This method is overridden to remove WebControl's default <span> container
        }

        public virtual string GenerateHtml(string path)
        {
            throw new NotImplementedException();
        }
    }
}
