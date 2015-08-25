using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;

namespace SPMin.Controls
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:CssRegistration runat=server></{0}:CssRegistration>")]
    public class CssRegistration : AssetRegistrationControl
    {
        private bool _addToHead = true;

        [Category("Settings")]
        public override bool AddToHead
        {
            get { return _addToHead; }
            set { _addToHead = value; }
        }

        public override string GenerateHtml(string path)
        {
            return String.Format("<link rel='stylesheet' href='{0}' type='text/css' />",
                HttpUtility.HtmlEncode(path));
        }
    }
}
