using SPMin.Minifiers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SPMin
{
    public class FileMinifier
    {
        private IMinifier _minifier;
        private string fileExtension;
        private string content;

        public FileMinifier(string fileExtension, string content)
        {
            this.fileExtension = fileExtension;
            if (Minifier == null)
                throw new Exception(String.Format("No minifier found for {0} extension", fileExtension));

            this.content = content;
        }

        private IMinifier Minifier
        {
            get
            {
                if (_minifier == null)
                {
                    switch (fileExtension.ToLower())
                    {
                        case "css":
                            _minifier = new CssMinifier();
                            break;

                        case "js":
                            _minifier = new JavaScriptMinifier();
                            break;
                    }
                }

                return _minifier;
            }
        }

        public string Minify()
        {
            return Minifier.Minify(content);
        }

        public byte[] MinifyAsByteArray()
        {
            return Encoding.UTF8.GetBytes(Minify());
        }
    }
}
