using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Dto.Request
{
    public class HumanInfoBlackSearchCondition
    {
        [StringLength(20)]
        public string KeyWord { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
