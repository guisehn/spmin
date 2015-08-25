using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;

namespace SPMin.Controls
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:JsRegistration runat=server></{0}:JsRegistration>")]
    public class JsRegistration : AssetRegistrationControl
    {
        private bool _addToHead = false;

        [Category("Settings")]
        public override bool AddToHead
        {
            get { return _addToHead; }
            set { _addToHead = value; }
        }

        public override string GenerateHtml(string path)
        {
            return String.Format("<script src='{0}' type='text/javascript'></script>",
                HttpUtility.HtmlEncode(path));
        }
    }
}
