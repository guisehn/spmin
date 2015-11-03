using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System.Collections.Generic;
using System.Linq;

namespace SPMin.SPMinEventReceiver
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class SPMinEventReceiver : SPItemEventReceiver
    {
        private object _lock = new object();

        /// <summary>
        /// An item was added.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            base.ItemAdded(properties);

            CreateOrUpdateMinifiedFile(properties);
        }

        /// <summary>
        /// An item was updated.
        /// </summary>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            base.ItemUpdated(properties);

            lock (_lock)
            {
                SPFile minifiedFile = CreateOrUpdateMinifiedFile(properties);

                if (minifiedFile != null)
                    PerformCheckIn(properties, properties.ListItem.File, minifiedFile);
            }
        }

        /// <summary>
        /// An item was checked out.
        /// </summary>
        public override void ItemCheckedOut(SPItemEventProperties properties)
        {
            base.ItemCheckedOut(properties);

            RunForMainFile(properties, (fileNameParser, targetFolder, mainFile, minifiedFile) =>
            {
                if (minifiedFile != null && minifiedFile.CheckedOutByUser == null)
                    minifiedFile.CheckOut();
            });
        }

        /// <summary>
        /// An item is being deleted
        /// </summary>
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            base.ItemDeleting(properties);

            lock (_lock)
            {
                RunForMainFile(properties, (fileNameParser, targetFolder, mainFile, minifiedFile) =>
                {
                    if (minifiedFile != null)
                    {
                        minifiedFile.Delete();

                        var path = FileUtilities.RemoveStyleLibraryFromPath(mainFile.Url);
                        var fileHashDictionary = new FileHashDictionary(properties.ListItem.Web.Site);
                        fileHashDictionary.Remove(path);
                    }
                });
            }
        }

        /// <summary>
        /// An item was unchecked out.
        /// </summary>
        public override void ItemUncheckedOut(SPItemEventProperties properties)
        {
            base.ItemUncheckedOut(properties);

            RunForMainFile(properties, (fileNameParser, targetFolder, mainFile, minifiedFile) =>
            {
                if (minifiedFile != null && minifiedFile.CheckedOutByUser.ID == properties.Web.CurrentUser.ID)
                    minifiedFile.UndoCheckOut();
            });
        }

        private void PerformCheckIn(SPItemEventProperties properties, SPFile mainFile, SPFile minifiedFile)
        {
            CheckInType checkInType = new CheckInTypeDetector(properties).Detect();
            if (checkInType != CheckInType.MajorVersion)
                return;

            var hash = FileUtilities.GetMD5Hash(minifiedFile.OpenBinary());
            var path = FileUtilities.RemoveStyleLibraryFromPath(mainFile.Url);
            var fileNameParser = new FileNameParser(path);

            minifiedFile.CheckIn("", SPCheckinType.MajorCheckIn);
            minifiedFile.Item["Name"] = fileNameParser.GenerateMinifiedVersionFileName(hash);
            minifiedFile.Item.SystemUpdate();

            var fileHashDictionary = new FileHashDictionary(properties.ListItem.Web.Site);
            fileHashDictionary.Update(path, hash);
        }

        private SPFile CreateOrUpdateMinifiedFile(SPItemEventProperties properties)
        {
            SPFile returnValue = null;

            RunForMainFile(properties, (fileNameParser, targetFolder, mainFile, minifiedFile) =>
            {
                var reader = new FileContentParser(mainFile);
                var content = reader.GetCompiledContent();
                var minifier = new FileMinifier(fileNameParser.FileExtension, content);

                if (minifiedFile != null)
                {
                    if (minifiedFile.CheckedOutByUser == null)
                        minifiedFile.CheckOut();

                    minifiedFile.SaveBinary(minifier.MinifyAsByteArray());
                }
                else
                {
                    var fileHashDictionary = new FileHashDictionary(properties.ListItem.Web.Site);
                    var fileHash = fileHashDictionary[fileNameParser.FilePath];
                    var minifiedFileName = fileNameParser.GenerateMinifiedVersionFileName(fileHash);

                    string fileUrl = String.Format("{0}/{1}", targetFolder.Url, minifiedFileName);
                    minifiedFile = targetFolder.Files.Add(fileUrl, minifier.MinifyAsByteArray());
                }

                returnValue = minifiedFile;
            });

            return returnValue;
        }

        private void RunForMainFile(SPItemEventProperties properties, Action<FileNameParser, SPFolder, SPFile, SPFile> action)
        {
            SPListItem item = properties.ListItem;
            if (item.Folder != null)
                return;

            var url = FileUtilities.RemoveStyleLibraryFromPath(item.File.Url);
            var fileNameParser = new FileNameParser(url);
            SPList styleLibrary = properties.ListItem.ParentList;
            SPFolder spminFolder = EnsureSPMinFolder(styleLibrary);

            if (fileNameParser.ShouldBeMinified)
            {
                var fileHashDictionary = new FileHashDictionary(properties.ListItem.Web.Site);
                var fileHash = fileHashDictionary[url];

                SPFile minifiedFile = FileUtilities.GetFile(spminFolder, fileNameParser.GenerateMinifiedVersionFileName(fileHash));
                action(fileNameParser, spminFolder, item.File, minifiedFile);
            }
        }

        private SPFolder EnsureSPMinFolder(SPList list)
        {
            var rootFolder = list.RootFolder;
            SPFolder folder = rootFolder.SubFolders.OfType<SPFolder>()
                .Where(f => f.Name.Equals(Constants.SPMinFolderName, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();

            if (folder == null)
                folder = rootFolder.SubFolders.Add(Constants.SPMinFolderName);

            return folder;
        }
    }
}