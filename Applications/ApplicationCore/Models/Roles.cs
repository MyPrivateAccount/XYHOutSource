using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationCore.Models
{
    public class Roles : IdentityRole
    {
        [MaxLength(127)]
        public string OrganizationId { get; set; }
    }
}
