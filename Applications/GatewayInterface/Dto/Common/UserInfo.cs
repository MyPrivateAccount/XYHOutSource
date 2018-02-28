using System;
using System.Collections.Generic;
using System.Text;

namespace GatewayInterface.Dto
{
    public class UserInfo
    {
        public string Id { get; set; }

        public string UserName { get; set; }
        public string KeyWord { get; set; }
        public string OrganizationId { get; set; }

        public string OrganizationName { get; set; }

    }
}
