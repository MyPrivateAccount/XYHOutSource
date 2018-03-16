using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHShopsPlugin.Models
{
    public class CustomerNotShops
    {
        [MaxLength(127)]
        public string CustomerId { get; set; }

        [MaxLength(127)]
        public string ShopsId { get; set; }

        [MaxLength(127)]
        public string UserId { get; set; }

        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 类型 是商铺还是楼盘 0商铺 1楼盘
        /// </summary>
        public int? Type { get; set; }
    }
}
