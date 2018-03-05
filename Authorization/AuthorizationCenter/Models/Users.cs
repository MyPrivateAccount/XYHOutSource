using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthorizationCenter.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class Users : IdentityUser
    {
        [MaxLength(127)]
        public string OrganizationId { get; set; }

        [MaxLength(127)]
        public string FilialeId { get; set; }

        [MaxLength(256)]
        public string TrueName { get; set; }

        [MaxLength(256)]
        public string Position { get; set; }

        [MaxLength(127)]
        public string WXOpenId { get; set; }

        [MaxLength(512)]
        public string Avatar { get; set; }

        public bool IsDeleted { get; set; }
        [NotMapped]
        public string Filiale { get; set; }
        [NotMapped]
        public string Organization { get; set; }
        [NotMapped]
        public string CityCode { get; set; }
    }
}
