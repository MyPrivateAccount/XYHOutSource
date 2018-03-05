using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Dto
{
    public class UserLoginLogSearchCondition
    {
        public string KeyWord { get; set; }
        public List<string> UserIds { get; set; }
        public List<string> OrganizationIds { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
