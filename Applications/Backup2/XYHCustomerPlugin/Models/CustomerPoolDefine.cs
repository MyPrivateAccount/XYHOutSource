using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    public class CustomerPoolDefine : TraceUpdateBase
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [MaxLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 公客池名字
        /// </summary>
        [MaxLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        [MaxLength(127)]
        public string DepartmentId { get; set; }
        /// <summary>
        /// 父级Id
        /// </summary>
        [MaxLength(127)]
        public string ParentId { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public int ExpireDay { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(1000)]
        public string Desc { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
