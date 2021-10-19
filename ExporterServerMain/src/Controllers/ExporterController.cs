using ExporterShared.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

            try
            {
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
            catch (Exception e)
            {
                Logger.Instance.AddLog(e.Message);
                return BadRequest(e.Message);
            }
        }

        [DisableRequestSizeLimit]
        [HttpPost]
        public IActionResult OnPost_Files([FromBody] FileList fileList)
        {
            Logger.Instance.AddLog("服务器开始接收文件 ...");
            string targetPath = fileList.TargetPath;
            try
            {
                foreach (FileData filedata in fileList.Files)
                {
                    string fullPath = Path.Combine(targetPath, filedata.RelativePath);
                    string fullDirectoryPath = Path.GetDirectoryName(fullPath);
                    if (!Directory.Exists(fullDirectoryPath))
                    {
                        Directory.CreateDirectory(fullDirectoryPath);
                    }

                    byte[] bytes = Convert.FromBase64String(filedata.Data);
                    System.IO.File.WriteAllBytes(fullPath, bytes);

                    Logger.Instance.AddLog(string.Format("已更新 {0}", filedata.RelativePath));
                }
                Logger.Instance.AddLog(String.Format("服务器已更新 {0} 个文件！", fileList.Files.Length));


                return Ok();
            }
            catch(Exception e)
            {
                Logger.Instance.AddLog(e.Message);
                return BadRequest(e.Message);
            }
        }

        [DisableRequestSizeLimit]
        [HttpPost]
        public async Task<IActionResult> OnPost_Command([FromBody] Command command)
        {
            command.OnProgress += (int execTime) =>
            {
                Logger.Instance.AddLog(String.Format("[服务器命令执行] 已执行{0}秒", execTime));
            };
            Logger.Instance.AddLog("服务器开始接收命令 ...");
            try
            {
                await command.Execute();
                return Ok();
            }
            catch (Exception e)
            {
                Logger.Instance.AddLog(e.Message);
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IActionResult OnGet_Logs()
        {
            return Ok(Logger.Instance.GetLogs());
        }
    }
}