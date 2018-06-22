using System;
using System.Collections.Generic;
using System.Text;

namespace XYHContractPlugin.Dto.Request
{
    public class ContractSearchRequest
    {
        /// <summary>
        /// 用户条件
        /// </summary>
        /// 
        public int SearchType { get; set; }
        public string KeyWord { get; set; }
        public int Discard { get; set; }
        public int OverTime { get; set; }
        public int Follow { get; set; }
        public int CheckStatu { get; set; }
        public string Organizate { get; set; }
        /// <summary>
        /// 创建时间开始
        /// </summary>
        public DateTime? CreateDateStart { get; set; }
        /// <summary>
        /// 创建时间结束
        /// </summary>
        public DateTime? CreateDateEnd { get; set; }
        public List<string> ProjectTypes { get; set; }
        public List<string> ContractTypes { get; set; }
        public int OrderRule { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }
}
