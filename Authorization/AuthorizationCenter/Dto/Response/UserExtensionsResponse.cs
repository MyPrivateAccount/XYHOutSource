using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Dto
{
    public class UserExtensionsResponse
    {
        
        public string UserId { get; set; }

       
        public string ParName { get; set; }

       
        public string ParValue { get; set; }


        
        public string ParValue2 { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
