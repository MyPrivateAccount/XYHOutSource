using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto
{
    public class HumanInfoResponse
    {
       
        public string ID { get; set; }
        
        public string UserID { get; set; }
      
        public string Name { get; set; }

        public string DepartmentId { get; set; }

        public string Position { get; set; }
       
        public PositionInfoResponse PositionInfo { get; set; }
    }
}
