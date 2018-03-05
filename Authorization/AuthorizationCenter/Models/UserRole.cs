using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorizationCenter.Models
{
    public class UserRole
    {
        [MaxLength(127)]
        public string UserId { get; set; }
        [MaxLength(127)]
        public string RoleId { get; set; }

        //public bool IsAgent { get; set; }
        [NotMapped]
        public Users Users { get; set; }
        [NotMapped]
        public Roles Roles { get; set; }
    }
}
