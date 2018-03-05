using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizationCenter.Dto
{
    public class BDFaceRegisterRequest
    {
        public string uid { get; set; }

        public string user_info { get; set; }

        public string image { get; set; }

        public List<string> groups { get; set; }

        public string actionType { get; set; }
       
    }
}
