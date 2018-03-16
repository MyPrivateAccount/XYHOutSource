using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Dto
{
    public class CustomerPoolDefineRequest
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [StringLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 公客池名字
        /// </summary>
        [StringLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        [StringLength(127)]
        public string DepartmentId { get; set; }
        /// <summary>
        /// 父级Id
        /// </summary>
        [StringLength(127)]
        public string ParentId { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public int ExpireDay { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(1000)]
        public string Desc { get; set; }
    }
}
