
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace XYHContractPlugin.Models
{
    /// <summary>
    /// 甲方公司信息
    /// </summary>
    public class CompanyAInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        [MaxLength(64)]
        public string Type { get; set; }
        [MaxLength(256)]
        public string Address { get; set; }
        [MaxLength(256)]
        public string Name { get; set; }
        [MaxLength(32)]
        public string PhoneNum { get; set; }
        [MaxLength(127)]
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }
        public bool IsDelete { get; set; }
        [MaxLength(127)]
        public string DeleteUser { get; set; }
        public DateTime? DeleteTime { get; set; }
        [MaxLength(256)]
        public string Ext1 { get; set; }
        [MaxLength(256)]
        public string Ext2 { get; set; }
    }
}