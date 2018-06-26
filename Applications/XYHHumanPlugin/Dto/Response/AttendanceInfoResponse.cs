using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Dto.Response
{
    public class AttendanceInfoResponse
    {
        public string ID { get; set; }
        public string UserID { get; set; }
        public DateTime? Date { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public int Normal { get; set; }
        public string NormalDate { get; set; }
        public int Relaxation { get; set; }
        public string RelaxationDate { get; set; }
        public int Matter { get; set; }
        public string MatterDate { get; set; }
        public int Illness { get; set; }
        public string IllnessDate { get; set; }
        public int Annual { get; set; }
        public string AnnualDate { get; set; }
        public int Marry { get; set; }
        public string MarryDate { get; set; }
        public int Funeral { get; set; }
        public string FuneralDate { get; set; }
        public int Late { get; set; }
        public string LateDate { get; set; }
        public int Absent { get; set; }
        public string AbsentDate { get; set; }
    }

    public class AttendanceSettingInfoResponse
    {
        public int Type { get; set; }
        /// <summary>
        /// 次数
        /// </summary>
        public int Times { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Money { get; set; }
    }

}
