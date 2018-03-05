using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizationCenter.Dto
{
    public class BDFaceVerifyRequest
    {
        public string image { get; set; }

        public string uid { get; set; }

        public List<string> groupIds { get; set; }

        public int topNum { get; set; }

        public List<string> extFileds { get; set; }
   
    }
}
