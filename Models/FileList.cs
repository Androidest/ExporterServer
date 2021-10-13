namespace ExporterShared.Models
{
    public class FileList
    {
        public string TargetPath { get; set; }
        public FileData[] Files { get; set; }

        public FileList()
        {
            TargetPath = "";
            Files = null;
        }

        public FileList(string targetPath, FileData[] fileDatas)
        {
            TargetPath = targetPath;
            Files = fileDatas;
        }
    }
}
