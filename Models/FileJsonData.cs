using System;
using System.Collections.Generic;
using System.Text;

namespace ExporterServer.Models
{
    public class FileJsonData
    {
        public string ExternalPath { get; set; }
        public string InternalPath { get; set; }
        public string Filename { get; set; }
        public string FileContent { get; set; }
    }
}
