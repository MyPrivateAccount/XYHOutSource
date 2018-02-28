using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Organizations
    {
        [Key]
        [MaxLength(127)]
        public virtual string Id { get; set; }

        [MaxLength(255)]
        public virtual string OrganizationName { get; set; }

        [MaxLength(255)]
        public virtual string Address { get; set; }

        [MaxLength(127)]
        public virtual string ParentId { get; set; }

        [MaxLength(255)]
        public virtual string Type { get; set; }

        public int PoolDay { get; set; }
    }
}
