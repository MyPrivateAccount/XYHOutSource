using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace XYHBaseDataPlugin.Models
{
    public class DictionaryDefine
    {
        [MaxLength(127)]
        public string GroupId { get; set; }
        [MaxLength(255)]
        public string Key { get; set; }
        [MaxLength(255)]
        public string Value { get; set; }
        public int Order { get; set; }
        [MaxLength(255)]
        public string Ext1 { get; set; }
        [MaxLength(255)]
        public string Ext2 { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(127)]
        public string DeleteUser { get; set; }
        public DateTime? DeleteTime { get; set; }
        [MaxLength(127)]
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }
        [MaxLength(127)]
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
