using ExporterServer.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace ExporterServer.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ExporterController : ControllerBase
    {
        [DisableRequestSizeLimit]
        [HttpPost]
        public IActionResult JsonPost([FromBody] FileJsonData data)
        {
            string path = Path.Combine(data.ExternalPath,data.InternalPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                path = Path.Combine(path, data.Filename);
                byte[] bytes = Convert.FromBase64String(data.FileContent);
                System.IO.File.WriteAllBytes(path, bytes);
                Console.WriteLine(data.Filename);
            }

            return Ok();
        }

    }
}