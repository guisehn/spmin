using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SPMin
{
    public class FileNameParser
    {
        private const string FileNameSuffix = ".spm";
        private readonly Regex FileNameSuffixRegex = new Regex("^(.*?)/([^/]+)" + Regex.Escape(FileNameSuffix) + @"\.(js|css)$",
            RegexOptions.Compiled);

        private string filePath;
        private Match match;

        public FileNameParser(string filePath)
        {
            this.filePath = filePath;
            this.match = FileNameSuffixRegex.Match(filePath);
        }

        public bool ShouldBeMinified
        {
            get
            {
                return match.Success;
            }
        }

        public string FileDirectory
        {
            get
            {
                return match.Groups[1].Value;
            }
        }

        public string FileNamePrefix
        {
            get
            {
                return match.Groups[2].Value;
            }
        }

        public string FileExtension
        {
            get
            {
                return match.Groups[3].Value;
            }
        }

        public string MinifiedVersionFileName
        {
            get
            {
                return String.Format("{0}-{1}.{2}", FileDirectory.Trim('/').Replace('/', '-'),
                    FileNamePrefix, FileExtension);
            }
        }
    }
}
