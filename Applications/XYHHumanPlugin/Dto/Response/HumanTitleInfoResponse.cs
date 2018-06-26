using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Dto.Response
{
    public class HumanTitleInfoResponse
    {

        /// <summary>
        /// 人事Id
        /// </summary>
        [StringLength(127)]
        public string HumanId { get; set; }

        /// <summary>
        /// 职称名称
        /// </summary>
        [StringLength(50)]
        public string TitleName { get; set; }

        /// <summary>
        /// 获得时间
        /// </summary>
        public DateTime GetTitleTime { get; set; }

    }
}
