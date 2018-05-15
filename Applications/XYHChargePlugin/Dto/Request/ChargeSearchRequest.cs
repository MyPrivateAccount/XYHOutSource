using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto.Request
{
    public class ChargeSearchRequest
    {
        /// <summary>
        /// 用户条件
        /// </summary>
        /// 
        public int SearchType { get; set; }//0是自己 1是全部
        public string KeyWord { get; set; }

        /// <summary>
        /// 离职
        /// </summary>
        /// 
        public int CheckStatu { get; set; }
        public int ChargePrice { get; set; }
        public string Organizate { get; set; }
        public int SearchTimeType { get; set; }
        public DateTime? CreateDateStart { get; set; }
        public DateTime? CreateDateEnd { get; set; }

        public int OrderRule { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }
}
