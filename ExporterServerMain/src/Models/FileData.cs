using System;
using System.IO;
using System.Security.Cryptography;

namespace ExporterShared.Models
{
    public class FileData
    {
        public string RelativePath { get; set; }
        public string Data { get; set; } = "";

        public FileData()
        {
            RelativePath = "";
            Data = "";
        }

        public FileData(string relativePath, string data)
        {
            RelativePath = relativePath;
            Data = data;
        }

        public static FileData CreateFileHash(string fullPath, string relativePath)
        {
            using (MD5 md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fullPath))
                {
                    string hashData = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                    return new FileData(relativePath, hashData);
                }
            }
        }

        public static FileData CreateFile(string parentPath, string relativePath)
        {
            string fullPath = Path.Combine(parentPath, relativePath);
            string contentData = Convert.ToBase64String(File.ReadAllBytes(fullPath));
            return new FileData(relativePath, contentData);
        }

        public void ToJsonFile(string parentPath)
        {
            // if parentPath is a file, not folder
            if (File.Exists(parentPath))
            {
                Data = Convert.ToBase64String(File.ReadAllBytes(parentPath));
            }
            else
            {
                string fullPath = Path.Combine(parentPath, RelativePath);
                Data = Convert.ToBase64String(File.ReadAllBytes(fullPath));
            }
        }
    }
}
