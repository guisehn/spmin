using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SPMin
{
    class CssRelativePathResolver
    {
        private string filePath;
        private string code;

        public CssRelativePathResolver(string filePath, string code)
        {
            this.filePath = filePath;
            this.code = code;
        }

        public string Resolve()
        {
            string fileDirectory = GetFileDirectory(filePath);

            return Regex.Replace(code, @"url\((.*?)\)", (Match match) =>
            {
                string value = Regex.Replace(match.Groups[1].Value, @"^['""](.*?)['""]$", "$1");

                if (Regex.IsMatch(value, @"^\w+:") || value.StartsWith("/"))
                    return match.Groups[0].Value;

                string finalPath = ".." + SimplifyPath(String.Format("{0}/{1}", fileDirectory, value));

                if (finalPath.Contains(" "))
                {
                    string escapedPath = finalPath.Replace("'", @"\'");
                    return String.Format("url('{0}')", escapedPath);
                }
                else
                {
                    return String.Format("url({0})", finalPath);
                }
            });
        }

        private string SimplifyPath(string path)
        {
            if (path.StartsWith("./"))
                path = path.Substring(2);

            var finalPath = new Stack<string>();
            var parts = path.Split('/');

            foreach (var part in parts)
            {
                if (part == "..")
                {
                    if (finalPath.Any())
                        finalPath.Pop();
                }
                else
                {
                    finalPath.Push(part);
                }
            }

            return String.Join("/", finalPath.Reverse().ToArray()).Replace("/./", "/");
        }

        private string GetFileDirectory(string filePath)
        {
            var path = Regex.Replace(filePath, "(.*?)/style library/(.*?)", "$2", RegexOptions.IgnoreCase);
            var parts = path.Split('/');

            return (parts.Count() > 1) ? '/' + String.Join("/", parts.Take(parts.Length - 1).ToArray()) : "";
        }
    }
}
