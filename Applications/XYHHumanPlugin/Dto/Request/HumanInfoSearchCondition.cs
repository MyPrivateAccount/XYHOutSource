using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYHHumanPlugin.Models;

namespace XYHHumanPlugin.Dto.Request
{
    public class HumanInfoSearchCondition
    {
        /// <summary>
        /// 关键字
        /// </summary>
        [StringLength(32)]
        public string KeyWord { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public List<StaffStatus> StaffStatuses { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        public string DepartmentId { get; set; }
        /// <summary>
        /// 生日开始
        /// </summary>
        public DateTime? BirthdayStart { get; set; }
        /// <summary>
        /// 生日结束
        /// </summary>
        public DateTime? BirthdayEnd { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
