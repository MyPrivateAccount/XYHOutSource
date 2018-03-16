using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Dto.Request
{
    public class AgainBeltLookRequest
    {
        [StringLength(127,ErrorMessage = "Id最长位数为127")]
        public string TransactionsId { get; set; }
        
        public DateTime ExpectedTime { get; set; }
    }
}
