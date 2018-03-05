using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Dto
{
    public class HavePermissionResponse
    {
        public string PermissionItem { get; set; }
        public bool IsHave { get; set; }
    }
}
