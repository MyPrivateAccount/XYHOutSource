using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace XYHBaseDataPlugin.Models
{
    public class DictionaryGroup
    {
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string Desc { get; set; }
        [MaxLength(127)]
        public string ValueType { get; set; }
        public bool HasExt1 { get; set; }
        public bool HasExt2 { get; set; }
        [MaxLength(255)]
        public string Ext1Desc { get; set; }
        [MaxLength(255)]
        public string Ext2Desc { get; set; }
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
