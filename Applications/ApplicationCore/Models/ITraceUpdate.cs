using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models
{
    public interface ITraceUpdate 
    {
      
        string CreateUser { get; set; }
        DateTime? CreateTime { get; set; }
       
        string UpdateUser { get; set; }
        DateTime? UpdateTime { get; set; }

      
        string DeleteUser { get; set; }
        DateTime? DeleteTime { get; set; }
    }
}
