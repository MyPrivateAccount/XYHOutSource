using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHShopsPlugin.Dto
{
    public class ShopSearchCondition
    {
        [StringLength(20)]
        public string KeyWord { get; set; }

        public List<string> BuildingIds { get; set; }

        public List<int?> ExamineStatus { get; set; }

        public List<string> SaleStatus { get; set; }



        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
