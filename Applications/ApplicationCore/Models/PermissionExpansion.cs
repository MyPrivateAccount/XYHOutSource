using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class PermissionExpansion
    {
        [MaxLength(127)]
        public virtual string UserId { get; set; }

        [MaxLength(256)]
        public virtual string UserName { get; set; }

        [MaxLength(127)]
        public virtual string ApplicationId { get; set; }

        [MaxLength(256)]
        public virtual string ApplicationName { get; set; }

        [MaxLength(127)]
        public virtual string OrganizationId { get; set; }

        [MaxLength(256)]
        public virtual string OrganizationName { get; set; }

        [MaxLength(127)]
        public virtual string PermissionId { get; set; }

        [MaxLength(256)]
        public virtual string PermissionName { get; set; }

        public virtual DateTime UpdateTime { get; set; }
    }
}
