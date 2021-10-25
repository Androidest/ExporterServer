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
        private void RemoveSrcDeletedFiles(string targetPath, Dictionary<string, bool> sourcePathDict)
        {
            var options = new EnumerationOptions();
            options.RecurseSubdirectories = true;
            string[] paths = Directory.GetFiles(targetPath, "*", options);
            int removeCount = 0;

            for (int i = 0; i < paths.Length; ++i)
            {
                string localRelativePath = Path.GetRelativePath(targetPath, paths[i]).Replace('\\', '/');
                if (!sourcePathDict.ContainsKey(localRelativePath))
                {
                    System.IO.File.Delete(paths[i]);
                    Logger.Instance.AddLog(string.Format("[服务器已删除] {0}. {1}", ++removeCount, localRelativePath));
                }
            }
            Logger.Instance.AddLog(String.Format("服务器已删除 {0} 个文件！", removeCount));
        }

        [DisableRequestSizeLimit]
        [HttpPost]
        public IActionResult OnPost_FileHashes([FromBody] FileList fileList)
        {
            List<FileData> hashesToUpdate = new List<FileData>();
            string targetPath = fileList.TargetPath;
            Dictionary<string, bool> sourcePathDict = new Dictionary<string, bool>();

            try
            {
                foreach (FileData fileData in fileList.Files)
                {
                    sourcePathDict.Add(fileData.RelativePath.Replace('\\', '/'), false);
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

                //危险操作，有可能导致删除整个目录
                if (1 < fileList.Files.Length)
                {
                    RemoveSrcDeletedFiles(targetPath, sourcePathDict);
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

                    Logger.Instance.AddLog(string.Format("[服务器已更新] {0}", filedata.RelativePath));
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
            command.OnProgress += (int execTime, string output) =>
            {
                if (string.Empty != output)
                {
                    Logger.Instance.AddLog(String.Format("[服务器命令输出] {0}", output));
                }
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
                Logger.Instance.AddLog("[命令错误] 命令执行不成功！");
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