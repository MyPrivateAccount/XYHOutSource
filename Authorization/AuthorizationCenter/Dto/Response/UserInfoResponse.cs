using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Dto
{
    public class UserInfoResponse
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Organization { get; set; }
        public string FilialeId { get; set; }
        public string Filiale { get; set; }
        public string CityCode { get; set; }
        public string TrueName { get; set; }
        public string Position { get; set; }
        public string Avatar { get; set; }
        public string RoleId { get; set; }
    }
}
