using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorizationCenter.Dto
{
    public class BDFaceVerifyResponse : BDFaceResponseBase
    {
        public int result_num { get; set; }

        public List<float> result { get; set; }
    }
}
