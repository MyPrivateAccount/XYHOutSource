using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    /// <summary>
    /// 客户电话信息
    /// </summary>
    public class CustomerPhone : TraceUpdateBase
    {
        /// <summary>
        /// Id
        /// </summary>
        [MaxLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        [MaxLength(255)]
        public string Phone { get; set; }
        /// <summary>
        /// 是否主要电话
        /// </summary>
        public bool IsMain { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        [MaxLength(127)]
        public string CustomerId { get; set; }
        /// <summary>
        /// 删除
        /// </summary>
        public bool IsDeleted { get; set; }

        [NotMapped]
        public string UserId { get; set; }

        [NotMapped]
        public string DepartmentId { get; set; }

    }
}
