using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto
{
    public class FileScopeResponse
    {
        
        public string ReceiptId { get; set; }

        
        public string FileGuid { get; set; }

        
        public List<FileInfoResponse> FileList { get; set; }

        public FileItemResponse FileItem { get; set; }
    }
}
