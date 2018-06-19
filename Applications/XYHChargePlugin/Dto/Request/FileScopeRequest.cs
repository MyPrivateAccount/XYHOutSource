using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto
{
    public class FileScopeRequest
    {
        
        public string ReceiptId { get; set; }

        
        public string FileGuid { get; set; }

        
        public List<FileInfoRequest> FileList { get; set; }
    }
}
