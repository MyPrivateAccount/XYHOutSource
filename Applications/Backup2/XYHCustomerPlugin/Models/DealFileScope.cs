using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    public class DealFileScope : TraceUpdateBase
    {
        [MaxLength(127)]
        public string DealId { get; set; }
        [MaxLength(127)]
        public string FileGuid { get; set; }
        [MaxLength(255)]
        public string From { get; set; }

        /// <summary>
        /// 销售方式
        /// </summary>
        [MaxLength(11)]
        public DealFileType FileType { get; set; }

        public bool IsDeleted { get; set; }
    }

    public enum DealFileType
    {
        /// <summary>
        /// 图片
        /// </summary>
        Image = 1,

        /// <summary>
        /// 文档
        /// </summary>
        Docx = 2,

        /// <summary>
        /// 其他
        /// </summary>
        Other = 3
    }
}
