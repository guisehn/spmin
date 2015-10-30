using Microsoft.SharePoint;
using SPMin.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPMin
{
    public class FileHashDictionary
    {
        private const string PropertyName = "SPMinFileHashes";

        private SPWeb rootWeb;
        private SerializableDictionary dictionary;

        public FileHashDictionary(SPSite site)
        {
            this.rootWeb = site.RootWeb;
            Populate();
        }

        public string this[string filePath]
        {
            get
            {
                return (dictionary.ContainsKey(filePath)) ? dictionary[filePath] : null;
            }
        }

        public void Remove(string filePath)
        {
            dictionary.Remove(filePath);
            Save();
        }

        public void Update(string filePath, string fileHash)
        {
            dictionary[filePath] = fileHash;
            Save();
        }

        private void Save()
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (var site = new SPSite(rootWeb.Site.Url))
                {
                    var web = site.RootWeb;
                    web.AllowUnsafeUpdates = true;

                    try
                    {
                        web.AllProperties[PropertyName] = dictionary.ToString();
                        web.Update();
                    }
                    finally
                    {
                        web.AllowUnsafeUpdates = false;
                    }
                }
            });
        }

        private void Populate()
        {
            string serializedFileHashes = Convert.ToString(rootWeb.AllProperties[PropertyName]) ?? "";
            this.dictionary = new SerializableDictionary(serializedFileHashes);
        }
    }
}
