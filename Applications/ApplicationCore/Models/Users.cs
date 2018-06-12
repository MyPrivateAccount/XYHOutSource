using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationCore.Models
{
    //public class Users : IdentityUser
    public class Users 
    {
        [MaxLength(127)]
        public string Id { get; set; }

        [MaxLength(127)]
        public string UserName { get; set; }

        [MaxLength(127)]
        public string OrganizationId { get; set; }

        [MaxLength(127)]
        public string FilialeId { get; set; }

        [MaxLength(256)]
        public string TrueName { get; set; }

        //[MaxLength(127)]
        //public string Position { get; set; }

        [MaxLength(127)]
        public string WXOpenId { get; set; }

        [MaxLength(512)]
        public string Avatar { get; set; }

        [MaxLength(512)]
        public string PhoneNumber { get; set; }

        public bool IsDeleted { get; set; }
    }
}
