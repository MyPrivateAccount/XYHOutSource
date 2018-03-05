using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Dto
{
    public class UserInfoRequest
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        [StringLength(127)]
        public string OrganizationId { get; set; }
        [StringLength(127)]
        public string FilialeId { get; set; }
        [StringLength(256)]
        public string TrueName { get; set; }
        [StringLength(127)]
        public string Position { get; set; }
        [StringLength(512)]
        public string Avatar { get; set; }
    }
}
