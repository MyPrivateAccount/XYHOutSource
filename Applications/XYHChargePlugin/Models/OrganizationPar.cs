using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace XYHChargePlugin.Models
{
    /// <summary>
    /// 组织参数
    /// </summary>
    public class OrganizationPar
    {
        [MaxLength(127)]
        [Key]
        public string Id { get; set; }

        [MaxLength(64)]
        public string ParGroup { get; set; }

        [MaxLength(127)]
        public string OrganizationId { get; set; }

        [MaxLength(255)]
        public string ParValue1 { get; set; }

        [MaxLength(255)]
        public string ParValue2 { get; set; }

        [MaxLength(255)]
        public string ParValue3 { get; set; }

        [MaxLength(255)]
        public string Memo { get; set; }

        [MaxLength(127)]
        public string CreateUser { get; set; }

        public DateTime? CreateTime { get; set; }

        [MaxLength(127)]
        public string UpdateUser { get; set; }

        public DateTime? UpdateTime { get; set; }

      
    }
}
