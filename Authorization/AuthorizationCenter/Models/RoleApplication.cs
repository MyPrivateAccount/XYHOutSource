using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Models
{
    public class RoleApplication
    {
        [MaxLength(127)]
        public string RoleId { get; set; }

        [MaxLength(127)]
        public string ApplicationId { get; set; }

    }
}
