using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
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

        public static SPFile GetFile(SPFolder folder, string relativeFilePath)
        {
            var currentFolder = folder;
            var parts = relativeFilePath.Split('/');
            var pathFolders = parts.Take(parts.Length - 1);
            var fileName = parts.Last();

            try
            {
                foreach (var folderName in pathFolders)
                {
                    if (folderName == "..")
                        currentFolder = currentFolder.ParentFolder;
                    else
                        currentFolder = currentFolder.SubFolders.OfType<SPFolder>().FirstOrDefault(f => f.Name == folderName);

                    if (currentFolder == null || !currentFolder.Exists)
                        return null;
                }
            }
            catch (Exception exception)
            {
                var message = String.Format("Error on FileUtilities#GetFile for parameters \"{0}\", \"{1}\"\n{2}",
                    folder.ServerRelativeUrl, relativeFilePath, exception);
                SPMinLoggingService.LogError(message, TraceSeverity.Unexpected);
                return null;
            }

            return currentFolder.Files.OfType<SPFile>().FirstOrDefault(f => f.Name == fileName);
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
