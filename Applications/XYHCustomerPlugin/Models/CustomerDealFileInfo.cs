using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    public class CustomerDealFileInfo : TraceUpdateBase
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
        [MaxLength(64)]
        public string ProofType { get; set; }
        public bool IsDeleted { get; set; }
    }
}
