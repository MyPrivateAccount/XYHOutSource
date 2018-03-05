using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Models
{
    public class RolePermission
    {
        [MaxLength(127)]
        public string RoleId { get; set; }

        [MaxLength(127)]
        public string PermissionId { get; set; }

        [MaxLength(127)]
        public string OrganizationScope { get; set; }

        //public bool IsAgent { get; set; }
    }
}
