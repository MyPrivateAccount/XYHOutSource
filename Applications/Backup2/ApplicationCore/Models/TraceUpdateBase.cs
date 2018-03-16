using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Models
{
    public  class TraceUpdateBase : ITraceUpdate
    {
        [MaxLength(127)]
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }
        [MaxLength(127)]
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }

        [MaxLength(127)]
        public string DeleteUser { get; set; }
        public DateTime? DeleteTime { get; set; }

        [NotMapped]
       public SimpleUser CreateUserInfo { get; set; }

        [NotMapped]
        public SimpleUser UpdateUserInfo { get; set; }

        [NotMapped]
        public SimpleUser DeleteUserInfo { get; set; }
    }
}
