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
        private const string MinifiedFileNameSuffix = ".min";
        private readonly Regex FileNameSuffixRegex = new Regex("^(.*?)" + Regex.Escape(FileNameSuffix) + @"\.(js|css)$",
            RegexOptions.Compiled);

        private string fileName;
        private Match match;

        public FileNameParser(string fileName)
        {
            this.fileName = fileName;
            this.match = FileNameSuffixRegex.Match(fileName);
        }

        public bool ShouldBeMinified
        {
            get
            {
                return match.Success;
            }
        }

        public string FileNamePrefix
        {
            get
            {
                return match.Groups[1].Value;
            }
        }

        public string FileExtension
        {
            get
            {
                return match.Groups[2].Value;
            }
        }

        public string MinifiedVersionFileName
        {
            get
            {
                return String.Format("{0}{1}{2}.{3}", FileNamePrefix, FileNameSuffix, MinifiedFileNameSuffix, FileExtension);
            }
        }
    }
}
