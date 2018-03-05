using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Models
{
    public class Organization
    {
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }

        [MaxLength(256)]
        public string OrganizationName { get; set; }

        [MaxLength(256)]
        public string Type { get; set; }

        [MaxLength(50)]
        public string Phone { get; set; }

        [MaxLength(50)]
        public string Fax { get; set; }

        [MaxLength(256)]
        public string Address { get; set; }

        [MaxLength(127)]
        public string Manager { get; set; }

        [MaxLength(127)]
        public string Assistant { get; set; }

        [MaxLength(127)]
        public string LeaderManager { get; set; }

        [MaxLength(127)]
        public string Superiors { get; set; }

        [MaxLength(255)]
        public string City { get; set; }

        [MaxLength(127)]
        public string ParentId { get; set; }

        public int Sort { get; set; }

        public bool IsDeleted { get; set; }
        [NotMapped]
        public string FullName { get; set; }

    }
}
