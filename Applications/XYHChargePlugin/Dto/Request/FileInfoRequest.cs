using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHChargePlugin.Dto.Request
{
    public class FileInfoRequest
    {
        [StringLength(127)]
        public string FileGuid { get; set; }
        [StringLength(255)]
        public string From { get; set; }
        [StringLength(255)]
        public string WXPath { get; set; }
        [StringLength(255)]
        public string Source { get; set; }
        [StringLength(127)]
        public string SourceId { get; set; }
        [StringLength(127)]
        public string AppId { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(255)]
        public string FileExt { get; set; }

        [StringLength(64)]
        public string Driver { get; set; }
        [StringLength(255)]
        public string Group { get; set; }
    }
}
