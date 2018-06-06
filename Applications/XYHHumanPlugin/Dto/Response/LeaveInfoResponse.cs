using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Dto.Response
{
    public class LeaveInfoResponse
    {
        public string ID {get;set;}//这个不是数据库的，专门用来放human表的id
        public string IDCard { get; set; }
        public DateTime? LeaveTime { get; set; }
        public bool IsFormalities { get; set; }
        public bool IsReduceSocialEnsure { get; set; }
    }
}
