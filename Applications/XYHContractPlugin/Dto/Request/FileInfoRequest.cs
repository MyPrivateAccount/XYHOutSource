using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHContractPlugin.Dto.Request
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

        public string Ext1 { get; set; }
        public string Ext2 { get; set; }
    }

    public class FileItemRequest
    {
        public string FileGuid { get; set; }

        public string Icon { get; set; }

        public string Original { get; set; }

        public string Medium { get; set; }

        public string Small { get; set; }

        public string Group { get; set; }

        public string Ext1 { get; set; }
        public string Ext2 { get; set; }

    }
    public class FileUploadRequest
    {

        public List<FileInfoRequest> AddFileList { get; set; }
        public List<string> DeleteFileList { get; set; }

        public List<FileItemRequest> ModifyFileList { get; set; }
    }
}
