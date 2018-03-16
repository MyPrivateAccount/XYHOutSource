using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto
{
    public class CustomerLossRequest
    {
        [Required]
        [StringLength(127)]
        public string CustomerId { get; set; }
        /// <summary>
        /// 失效类型（已购、失效、暂停、外泄）
        /// </summary>
        public LossType LossTypeId { get; set; }
        /// <summary>
        /// 失效备注
        /// </summary>
        [StringLength(600)]
        public string LossRemark { get; set; }

        /// <summary>
        /// 是否保留以前报备信息
        /// </summary>
        public bool IsDeleteOldData { get; set; }
    }
}
