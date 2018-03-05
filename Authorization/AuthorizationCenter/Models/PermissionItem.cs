using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Models
{
    public class PermissionItem
    {
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }

        [MaxLength(127)]
        public string ApplicationId { get; set; }

        [MaxLength(127)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string Groups { get; set; }

    }
}
