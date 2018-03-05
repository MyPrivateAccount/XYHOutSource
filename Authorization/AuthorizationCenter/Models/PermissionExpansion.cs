using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Models
{
    public class PermissionExpansion
    {
        [MaxLength(127)]
        public string UserId { get; set; }

        [MaxLength(256)]
        public string UserName { get; set; }

        [MaxLength(127)]
        public string ApplicationId { get; set; }

        [MaxLength(256)]
        public string ApplicationName { get; set; }

        [MaxLength(127)]
        public string OrganizationId { get; set; }

        [MaxLength(256)]
        public string OrganizationName { get; set; }

        [MaxLength(127)]
        public string PermissionId { get; set; }

        [MaxLength(256)]
        public string PermissionName { get; set; }

        //public bool IsAgent { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
