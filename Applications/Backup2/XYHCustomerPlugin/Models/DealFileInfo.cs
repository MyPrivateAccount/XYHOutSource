using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    /// <summary>
    /// 成交文件信息
    /// </summary>
    public class DealFileInfo
    {
        [Key]
        public string Id { get; set; }

        public string WXPath { get; set; }

        public string AppId { get; set; }

        /// <summary>
        /// 文件Guid
        /// </summary>
        public string FileGuid { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string From { get; set; }

        public ProofType ProofType { get; set; }

    }
    /// <summary>
    /// 附件类型
    /// </summary>
    public enum ProofType
    {
        /// <summary>
        /// 带看确认书
        /// </summary>
        LookConfirm = 1,

        /// <summary>
        /// 客户身份证
        /// </summary>
        CustomerIdCard = 2,

        /// <summary>
        /// 购买意向书
        /// </summary>
        LetterIntent = 3,

        /// <summary>
        /// 其他
        /// </summary>
        Others = 4
    }
}
