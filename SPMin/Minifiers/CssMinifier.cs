using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SPMin.Minifiers
{
    public class CssMinifier : IMinifier
    {
        public string Minify(string content)
        {
            var minifier = new Microsoft.Ajax.Utilities.Minifier();
            return minifier.MinifyStyleSheet(content);
        }
    }
}
