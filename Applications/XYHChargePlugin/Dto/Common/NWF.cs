using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto.Common
{
    public class NWF
    {
        public HeaderType Header { get; set; }
        public BodyInfoType BodyInfo { get; set; }

    }

    public class HeaderType
    {
        public string TaskGuid { get; set; }
        public string ContentGuid { get; set; }
        public string Action { get; set; }
        public string SourceSystem { get; set; }
        public string TargetSystem { get; set; }
        public List<AttributeType> ExtraAttribute { get; set; }
    }

    public class BodyInfoType
    {
        public int Priority { get; set; }
        public string TaskName { get; set; }

        public List<FileInfoType> FileInfo { get; set; }

        public List<AttributeType> ExtraAttribute { get; set; }
    }

    public class FileInfoType
    {
        public string FilePath { get; set; }
        public string FileTypeId { get; set; }
        public int QualityType { get; set; }
        public string FileExt { get; set; }
        public string FileGuid { get; set; }
        public List<AttributeType> ExtraAttribute { get; set; }
    }

    public class AttributeType
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
