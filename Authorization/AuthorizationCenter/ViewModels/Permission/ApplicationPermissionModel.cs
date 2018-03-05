using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.ViewModels
{
    public class ApplicationPermissionModel
    {
        public string ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public List<PermissionModel> Permissions { get; set; }
    }


    public class PermissionModel
    {
        public string PermissionId { get; set; }
        public string PermissionName { get; set; }
        public string GroupName { get; set; }
        public List<OrganizationScopeModel> Organizations { get; set; }
    }

    public class OrganizationScopeModel
    {
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
    }

}
