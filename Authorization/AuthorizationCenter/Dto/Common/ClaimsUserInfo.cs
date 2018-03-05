using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Dto
{
    public class ClaimsUserInfo
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string KeyWord { get; set; }
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }


        public string grant_type { get; set; }

    }
}
