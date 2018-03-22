using System;
using System.Collections.Generic;
using System.Text;

namespace XYHContractPlugin.Dto.Response
{
    public class ContractAnnexResponse
    {
        public string ID { get; set; }
        public string ContractID { get; set; }
        public string FileGuid { get; set; }
        public string From { get; set; }
        public string Group { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
