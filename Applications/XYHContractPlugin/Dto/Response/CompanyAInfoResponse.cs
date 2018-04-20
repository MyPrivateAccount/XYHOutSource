using System;
using System.Collections.Generic;
using System.Text;

namespace XYHContractPlugin.Dto.Response
{
    public class CompanyAInfoResponse
    {

        public string ID { get; set; }
     
        public string Type { get; set; }
    
        public string Address { get; set; }
      
        public string Name { get; set; }
  
        public string PhoneNum { get; set; }

    
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }

        public bool IsDelete { get; set; }

        public string DeleteUser { get; set; }
        public DateTime? DeleteTime { get; set; }
        public string Ext1 { get; set; }

        public string Ext2 { get; set; }
    }
}
