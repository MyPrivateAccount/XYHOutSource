using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Dto
{
    public class AddUserToRolesExtRequest
    {
        public IEnumerable<string> Roles { get; set; }

        public IEnumerable<AgentRole> AgentRoles { get; set; }
    }

    public class AgentRole
    {
        public string Role { get; set; }
        public string Organization { get; set; }
    }

}
