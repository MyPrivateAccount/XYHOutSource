using System;
using System.Collections.Generic;
using System.Text;

namespace XYHContractPlugin.Dto.Response
{
    public class ContractModifyResponse
    {
        public string ID { get; set; }
        public string ContractID { get; set; }
        public int? Type { get; set; }

        public int? ExamineStatus { get; set; }
        public DateTime? ExamineTime { get; set; }
        public string ModifyPepole { get; set; }
        public DateTime? ModifyStartTime { get; set; }
        public string ModifyCheck { get; set; }
    }
}
