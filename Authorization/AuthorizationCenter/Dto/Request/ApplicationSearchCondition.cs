using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Dto
{
    public class ApplicationSearchCondition
    {
        public string KeyWords { get; set; }

        public List<string> Ids { get; set; }
        public List<string> ApplicationTypes { get; set; }
    }
}
