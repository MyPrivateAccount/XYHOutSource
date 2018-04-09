using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Dto.Response
{
    public class AttendanceInfoResponse
    {
        public string ID { get; set; }
        public DateTime? Time { get; set; }
        public string Name { get; set; }
        public string IDCard { get; set; }
        public string History { get; set; }
    }
}
