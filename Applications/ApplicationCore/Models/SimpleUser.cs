using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models
{
    public class SimpleUser
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public string PhoneNumber { get; set; }
    }
}
