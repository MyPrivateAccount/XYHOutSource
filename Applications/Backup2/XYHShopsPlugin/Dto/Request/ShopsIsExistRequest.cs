using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHShopsPlugin.Dto
{
    public class ShopsIsExistRequest
    {
        /// <summary>
        /// 
        /// </summary>
        [StringLength(127)]
        public string Id { get; set; }
        [StringLength(127)]
        public string BuildingId { get; set; }
        [StringLength(255)]
        public string BuildingNo { get; set; }
        [StringLength(255)]
        public string FloorNo { get; set; }
        [StringLength(255)]
        public string Number { get; set; }

        public string ShopId { get; set; }
    }
}
