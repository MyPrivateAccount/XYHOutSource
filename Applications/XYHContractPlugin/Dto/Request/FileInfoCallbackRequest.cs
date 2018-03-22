using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHContractPlugin.Dto.Request
{
    public class FileInfoCallbackRequest
    {
        [StringLength(127)]
        public string FileGuid { get; set; }
        [StringLength(255)]
        public string FilePath { get; set; }
        public string Size { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(255)]
        public string Type { get; set; }
        [StringLength(255)]
        public string FileExt { get; set; }
    }
}
