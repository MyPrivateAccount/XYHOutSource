using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHChargePlugin.Models
{
    public class FileScopeInfo
    {
        [Key]
        [MaxLength(127)]
        public string ReceiptId { get; set; }

        [Key]
        [MaxLength(127)]
        public string FileGuid { get; set; }

        [NotMapped]
        public List<FileInfo> FileList { get; set; }
    }
}
