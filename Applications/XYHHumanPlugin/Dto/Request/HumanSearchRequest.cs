using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Dto.Request
{
    public class HumanSearchRequest
    {
        /// <summary>
        /// 用户条件
        /// </summary>
        /// 
        public int SearchType { get; set; }
        public string KeyWord { get; set; }
        
        /// <summary>
        /// 离职
        /// </summary>
        /// 
        public int Departure { get; set; }
        public int OverTime { get; set; }
        public int CheckStatu { get; set; }
        public string Organizate { get; set; }
        
        public DateTime? CreateDateStart { get; set; }
        public DateTime? CreateDateEnd { get; set; }

        public int OrderRule { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }
}