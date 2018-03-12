using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    public class BeltLookFiles : TraceUpdateBase
    {
        /// <summary>
        /// 带看Id
        /// </summary>
        [MaxLength(127)]
        public string BeltLookId { get; set; }
        /// <summary>
        /// 文件guid
        /// </summary>
        [MaxLength(127)]
        public string FileGuid { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        [MaxLength(255)]
        public string From { get; set; }

        /// <summary>
        /// 文件类型（1图片2附件）
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }

}
