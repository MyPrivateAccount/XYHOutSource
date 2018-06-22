using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto
{
    public class ChargeSearchRequest
    {
        public string ReimburseDepartment { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsBackup { get; set; }

        public bool? IsPayment { get; set; }

        public string Keyword { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }
        ///// <summary>
        ///// 用户条件
        ///// </summary>
        ///// 
        //public int SearchType { get; set; }//0是自己 1是全部
        //public string KeyWord { get; set; }

        ///// <summary>
        ///// 离职
        ///// </summary>
        ///// 
        //public int CheckStatu { get; set; }
        //public int ChargePrice { get; set; }
        //public string Organizate { get; set; }
        //public int SearchTimeType { get; set; }
        //public DateTime? CreateDateStart { get; set; }
        //public DateTime? CreateDateEnd { get; set; }

        //public int OrderRule { get; set; }
        //public int pageIndex { get; set; }
        //public int pageSize { get; set; }
    }
}
