using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    public class CustomerFileScope
    {
        [MaxLength(127)]
        public string CustomerId { get; set; }
        [MaxLength(127)]
        public string FileGuid { get; set; }
        [MaxLength(255)]
        public string From { get; set; }
        /// <summary>
        /// 销售方式
        /// </summary>
        [MaxLength(11)]
        public DealFileType FileType { get; set; }

        [MaxLength(127)]
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }
        [MaxLength(127)]
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(127)]
        public string DeleteUser { get; set; }
        public DateTime? DeleteTime { get; set; }
    }
}
