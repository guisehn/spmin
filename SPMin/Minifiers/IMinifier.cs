using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPMin.Minifiers
{
    interface IMinifier
    {
        string Minify(string content);
    }
}
