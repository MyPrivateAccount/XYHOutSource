using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Dto
{
    public class RoleSearchCondition
    {
        public string KeyWords { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
