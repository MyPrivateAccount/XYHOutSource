﻿using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto.Response
{
    public class ReceiptInfoResponse
    {
        public string ID { get; set; }
        public string CostID { get; set; }
        public string ReceiptNumber { get; set; }
        public int? ReceiptMoney { get; set; }
        public string Comments { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }
    }
}