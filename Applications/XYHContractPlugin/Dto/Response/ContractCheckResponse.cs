﻿using System;
using System.Collections.Generic;
using System.Text;

namespace XYHContractPlugin.Dto.Response
{
    public class ContractCheckResponse
    {
        public string ID { get; set; }
        public string OriginID { get; set; }
        public string CheckID { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Current { get; set; }
    }

    public class ContractCheckInfoRequest
    {
        public string ContractID { get; set; }
        public string ModifyID { get; set; }
        public string CheckName { get; set; }
        public string Action { get; set; }
    }
}
