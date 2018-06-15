using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Dto
{
    [Serializable]
    public class UserInfo
    {
        public string Id { get; set; }

        public string UserName { get; set; }
        public string KeyWord { get; set; }
        public string OrganizationId { get; set; }
        public string PhoneNumber { get; set; }
        public string OrganizationName { get; set; }
    }
}
