using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    /// <summary>
    /// 公客池
    /// </summary>
    public class CustomerPool : TraceUpdateBase
    {
        /// <summary>
        /// Id(联合主键的话 至少要把ISDELETE带上 但是需要修改就不方便 )
        /// </summary>
        [Key]
        [MaxLength]
        public string Id { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public string DepartmentId { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// 加入时间
        /// </summary>
        public DateTime JoinDate { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

    }
}
