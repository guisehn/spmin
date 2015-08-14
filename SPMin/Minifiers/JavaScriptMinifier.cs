using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPMin.Minifiers
{
    public class JavaScriptMinifier : IMinifier
    {
        public string Minify(string content)
        {
            var minifier = new Microsoft.Ajax.Utilities.Minifier();
            return minifier.MinifyJavaScript(content);
        }
    }
}
