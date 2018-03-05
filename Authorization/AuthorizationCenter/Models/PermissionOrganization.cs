using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Models
{
    public class PermissionOrganization
    {
        [MaxLength(127)]
        public string OrganizationScope { get; set; }
        [MaxLength(127)]
        public string OrganizationId { get; set; }
    }
}
