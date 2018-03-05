using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuthorizationCenter.Models
{
    public class Roles : IdentityRole
    {
        [MaxLength(127)]
        public string OrganizationId { get; set; }

        [MaxLength(255)]
        public string Type { get; set; }
    }
}
