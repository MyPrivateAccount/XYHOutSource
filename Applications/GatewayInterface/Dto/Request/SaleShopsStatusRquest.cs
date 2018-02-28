using System;
using System.Collections.Generic;
using System.Text;

namespace GatewayInterface.Dto
{
    public class SaleShopsStatusRquest
    {
        public List<string> ShopsIds { get; set; }
        
        public string SaleStatus { get; set; }
        
        public string Mark { get; set; }

        public DateTime? LockTime { get; set; }
    }
}
