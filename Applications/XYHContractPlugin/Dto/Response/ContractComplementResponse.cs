using System;
using System.Collections.Generic;
using System.Text;

namespace XYHContractPlugin.Dto.Response
{
    public class ContractComplementResponse
    {
        public string ID { get; set; }
        public string ContractID { get; set; }
        public int? ContentID { get; set; }
        public string ContentInfo { get; set; }
    }
}
