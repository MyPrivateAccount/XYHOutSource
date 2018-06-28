using System;
using System.Collections.Generic;
using System.Text;
using XYHHumanPlugin.Models;

namespace XYHHumanPlugin.Dto.Response
{
    public class HumanInfoSearchResponse
    {

        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IDCard { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public Int16 Sex { get; set; }

        /// <summary>
        /// 职位名称
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 所属部门Id
        /// </summary>
        public string DepartmentId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        public string OrganizationFullName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public StaffStatus StaffStatus { get; set; }

        public decimal BaseWages { get; set; } = 0;

        /// <summary>
        /// 入职日期
        /// </summary>
        public DateTime EntryTime { get; set; }
        /// <summary>
        /// 转正日期？
        /// </summary>
        public DateTime? BecomeTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 是否参加社保
        /// </summary>
        public bool? IsHaveSocialSecurity { get; set; }
        /// <summary>
        /// 是否签订合同
        /// </summary>
        public bool? IsSignContracInfo { get; set; }
    }
}
