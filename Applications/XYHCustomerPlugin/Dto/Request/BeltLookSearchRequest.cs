using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Dto.Request
{
    public class BeltLookSearchRequest
    {
        [StringLength(127, ErrorMessage = "BeltHouse最大长度为127")]
        public string BeltHouse { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
