using Microsoft.SharePoint;
using System.Linq;
using System.Text;

namespace SPMin
{
    public class FileReader
    {
        private SPFile mainFile;
        private SPFolder parentFolder;
        private string mainContent;

        public string[] IncludedFiles { get; private set; }

        public FileReader(SPFile file)
        {
            this.mainFile = file;
            this.parentFolder = file.ParentFolder;
            this.mainContent = GetContent(mainFile);
            
            IncludedFiles = FileContentParser.GetIncludedFiles(mainContent);
        }

        public string GetCompiledContent()
        {
            var finalContent = new StringBuilder();

            foreach (string includedFileName in this.IncludedFiles)
            {
                SPFile includedFile = GetFile(includedFileName);

                if (includedFile != null)
                    finalContent.AppendLine(GetContent(includedFile));
            }

            finalContent.Append(mainContent);

            return finalContent.ToString();
        }

        private string GetContent(SPFile file)
        {
            return Encoding.UTF8.GetString(RemoveBOM(file.OpenBinary()));
        }

        private SPFile GetFile(string fileName)
        {
            return parentFolder.Files.OfType<SPFile>().FirstOrDefault(f => f.Name == fileName);
        }

        private byte[] RemoveBOM(byte[] bytes)
        {
            byte[] preamble = Encoding.UTF8.GetPreamble();

            if (bytes.Take(preamble.Length).SequenceEqual(preamble))
                bytes = bytes.Skip(preamble.Length).ToArray();

            return bytes;
        }
    }
}
