using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Dto.Request
{
    public class HumanPositionRequest
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [StringLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 职位名称
        /// </summary>
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 职位类型
        /// </summary>
        [MaxLength(255)]
        public string Type { get; set; }

        /// <summary>
        /// 所属分公司
        /// </summary>
        [MaxLength(127)]
        public string DepartmentId { get; set; }


    }
}
