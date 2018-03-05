using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Models
{
    public class UserLoginLog
    {
        [MaxLength(127)]
        public string UserId { get; set; }
        [MaxLength(256)]
        public string UserName { get; set; }
        [MaxLength(256)]
        public string TrueName { get; set; }
        [MaxLength(127)]
        public string OrganizationId { get; set; }
        public DateTime LoginTime { get; set; }
        [MaxLength(72)]
        public string LoginIp { get; set; }
        [MaxLength(255)]
        public string LoginApplication { get; set; }
    }
}
