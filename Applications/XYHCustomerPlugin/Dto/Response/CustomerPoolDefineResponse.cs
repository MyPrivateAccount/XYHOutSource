using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Dto
{
    public class CustomerPoolDefineResponse
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 公客池名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        public string DepartmentId { get; set; }
        /// <summary>
        /// 部门名字
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// 父级Id
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public int ExpireDay { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }
    }
}
