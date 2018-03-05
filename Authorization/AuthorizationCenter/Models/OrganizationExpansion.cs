using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Models
{
    /// <summary>
    /// 组织展开表
    /// </summary>
    public class OrganizationExpansion
    {
        [MaxLength(127)]
        public string OrganizationId { get; set; }
        [MaxLength(255)]
        public string OrganizationName { get; set; }
        [MaxLength(127)]
        public string SonId { get; set; }
        [MaxLength(255)]
        public string SonName { get; set; }
        /// <summary>
        /// 是否直接父子级关系
        /// </summary>
        public bool IsImmediate { get; set; }
        [MaxLength(255)]
        public string Type { get; set; }
        [MaxLength(255)]
        public string City { get; set; }
        [MaxLength(1024)]
        public string FullName { get; set; }
    }
}
