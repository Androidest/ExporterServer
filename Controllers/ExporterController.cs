using ExporterShared.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using Utils;

namespace ExporterServer.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ExporterController : ControllerBase
    {
        [DisableRequestSizeLimit]
        [HttpPost]
        public IActionResult OnPost_FileHashes([FromBody] FileList fileList)
        {
            List<FileData> hashesToUpdate = new List<FileData>();
            string targetPath = fileList.TargetPath;
            
            foreach (FileData fileData in fileList.Files)
            {
                string targetFilePath = Path.Combine(targetPath, fileData.RelativePath);

                // compare the client file hash and the server file hash,
                // if they are different indicates that this file sould be updated
                if (!System.IO.File.Exists(targetFilePath) ||
                    FileData.CreateFileHash(targetFilePath, "").Data != fileData.Data)
                {
                    fileData.Data = ""; // clear md5 hash
                    hashesToUpdate.Add(fileData);
                }
            }

            return new JsonResult(hashesToUpdate);
        }

        [DisableRequestSizeLimit]
        [HttpPost]
        public IActionResult OnPost_Files([FromBody] FileList fileList)
        {
            Logger.Instance.AddLog("Start updating files ...");

            string targetPath = fileList.TargetPath;
            foreach(FileData filedata in fileList.Files)
            {
                string fullPath = Path.Combine(targetPath, filedata.RelativePath);
                string fullDirectoryPath = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(fullDirectoryPath))
                {
                    Directory.CreateDirectory(fullDirectoryPath);
                }

                byte[] bytes = Convert.FromBase64String(filedata.Data);
                System.IO.File.WriteAllBytes(fullPath, bytes);

                Logger.Instance.AddLog(filedata.RelativePath);
            }

            Logger.Instance.AddLog(String.Format("Updated {0} files.", fileList.Files.Length));
            return Ok();
        }

        [HttpGet]
        public IActionResult OnGet_Logs()
        {
            return Ok(Logger.Instance.GetLogs());
        }
    }
}