using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SPMin
{
    public class FileInclusionParser
    {
        public static string[] GetIncludedFiles(string fileContent)
        {
            var includedFiles = new string[0];
            var match = Regex.Match(fileContent, @"^(?:\s+)?/\*(.*?)\*/", RegexOptions.Singleline);

            if (match.Success)
            {
                var lines = match.Groups[1].Value.Trim().Split('\n');
                var linesWithIncludes = lines.Where(line => line.Trim().StartsWith("*="));

                includedFiles = linesWithIncludes
                    .Select(l => l.Replace("*=", "").Trim())
                    .Where(l => l != "")
                    .Distinct()
                    .ToArray();
            }

            return includedFiles;
        }
    }
}
