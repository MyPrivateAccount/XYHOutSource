using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Dto.Response
{
    public class BlackInfoResponse
    {
        public string ID { get; set; }//这个不是数据库的，专门用来放human表的id
        public string IDCard { get; set; }
        public string Name { get; set; }
        public string Reason { get; set; }
    }
}
