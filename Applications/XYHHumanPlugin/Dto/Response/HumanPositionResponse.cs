using System;
using System.Collections.Generic;
using System.Text;
using XYHHumanPlugin.Models;

namespace XYHHumanPlugin.Dto.Response
{
    public class HumanPositionResponse
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 职位名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 职位类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 所属分公司
        /// </summary>
        public string DepartmentId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public ExamineStatusEnum ExamineStatus { get; set; } = ExamineStatusEnum.UnSubmit;
        public bool IsDeleted { get; set; }
    }
}
