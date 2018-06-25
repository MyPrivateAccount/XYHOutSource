using System;
using System.Collections.Generic;
using System.Text;
using XYHHumanPlugin.Dto.Response;

namespace XYHHumanPlugin.Dto.Request
{
    public class AttendenceSearchRequest
    {
        /// <summary>
        /// 用户条件
        /// </summary>
        /// 
        public string KeyWord { get; set; }
        public DateTime CreateDate { get; set; }

        public int OrderRule { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }
}
