using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Models
{
    public class UserExtensions
    {
        [MaxLength(127)]
        public string UserId { get; set; }

        [MaxLength(64)]
        public string ParName { get; set; }

        [MaxLength(512)]
        public string ParValue { get; set; }


        
        public string ParValue2 { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
