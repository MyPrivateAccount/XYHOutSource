using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto.Response
{
    public class ChargeInfoResponse
    {
        public string ID { get; set; }
        public string Department { get; set; }
        public string Note { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? PostTime { get; set; }
        public string CreateUser { get; set; }
        public string CreateUserName { get; set; }
    }
}
