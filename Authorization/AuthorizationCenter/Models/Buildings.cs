using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Models
{
    public class Buildings
    {
        [Key]
        [StringLength(127)]
        public string Id { get; set; }
    }
}
