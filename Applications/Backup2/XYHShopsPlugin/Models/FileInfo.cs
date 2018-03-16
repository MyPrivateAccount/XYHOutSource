using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHShopsPlugin.Models
{
    public class FileInfo
    {
        [MaxLength(127)]
        public string FileGuid { get; set; }
        public string Name { get; set; }
        [MaxLength(255)]
        public string Type { get; set; }
        [MaxLength(255)]
        public string FileExt { get; set; }
        public double Size { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        [MaxLength(1000)]
        public string Uri { get; set; }
        public string Summary { get; set; }
        [MaxLength(1000)]
        public string Ext1 { get; set; }
        [MaxLength(1000)]
        public string Ext2 { get; set; }

        [MaxLength(127)]
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }
        [MaxLength(127)]
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(127)]
        public string DeleteUser { get; set; }
        public DateTime? DeleteTime { get; set; }

        [MaxLength(64)]
        public string Driver { get; set; }
        [NotMapped]
        public string Group { get; set; }
    }
}
