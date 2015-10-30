using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SPMin
{
    public class FileUtilities
    {
        private const string StyleLibraryPath = "Style Library/";

        public static string RemoveStyleLibraryFromPath(string path)
        {
            if (path.StartsWith(StyleLibraryPath))
                path = path.Substring(StyleLibraryPath.Length);

            return path;
        }

        public static string RemoveDuplicatedSlashesFromPath(string path)
        {
            return Regex.Replace(path, "/{2,}", "/");
        }

        public static string GetDirectoryPathFromFilePath(string filePath)
        {
            string[] parts = filePath.Split('/');
            return String.Join("/", parts.Take(parts.Length - 1).ToArray());
        }

        public static SPFile GetFile(SPFolder folder, string fileName)
        {
            return folder.Files.OfType<SPFile>().FirstOrDefault(f => f.Name == fileName);
        }

        public static string GetMD5Hash(byte[] inputBytes)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("x2"));

            return sb.ToString();
        }
    }
}
