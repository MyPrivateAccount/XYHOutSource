using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class OrganizationExpansion
    {
        public virtual string OrganizationId { get; set; }

        public virtual string OrganizationName { get; set; }

        public virtual string SonId { get; set; }

        public virtual string SonName { get; set; }

        /// <summary>
        /// 是否直接父子级关系
        /// </summary>
        public bool IsImmediate { get; set; }
        public string Type { get; set; }
        public string City { get; set; }
        public string FullName { get; set; }

    }
}
