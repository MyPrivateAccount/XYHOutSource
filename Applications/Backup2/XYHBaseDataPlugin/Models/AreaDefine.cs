using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHBaseDataPlugin.Models
{
    public class AreaDefine
    {
        [Key]
        [MaxLength(127)]
        public string Code { get; set; }
        [MaxLength(255)]
        public string Level { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string Abbreviation { get; set; }
        [MaxLength(127)]
        public string ParentId { get; set; }
        public string Desc { get; set; }
        public int Order { get; set; }

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
    }
}
