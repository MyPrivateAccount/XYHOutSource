using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Dto.Response
{
    public class OpenIDResponse
    {
        public string UserID { get; set; }

        public string OpenID { get; set; }
    }
}
