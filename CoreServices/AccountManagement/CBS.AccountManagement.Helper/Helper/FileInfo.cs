namespace CBS.AccountManagement.Helper
{
    public class FileInfo
    {
        private string filePath;

        public FileInfo(string filePath)
        {
            this.filePath = filePath;
        }

        public string Src { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string FileType { get; set; }
    }
}