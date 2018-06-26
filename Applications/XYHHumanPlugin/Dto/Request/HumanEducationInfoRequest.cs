using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Dto.Request
{
    public class HumanEducationInfoRequest
    {
        /// <summary>
        /// 学历
        /// </summary>
        public string Education { get; set; }

        /// <summary>
        /// 所学专业
        /// </summary>
        public string Major { get; set; }

        /// <summary>
        /// 学习形式
        /// </summary>
        public string LearningType { get; set; }

        /// <summary>
        /// 毕业证书
        /// </summary>
        public string GraduationCertificate { get; set; }

        /// <summary>
        /// 入学时间
        /// </summary>
        public DateTime EnrolmentTime { get; set; }

        /// <summary>
        /// 毕业时间
        /// </summary>
        public DateTime GraduationTime { get; set; }

        /// <summary>
        /// 获得学位
        /// </summary>
        public string GetDegree { get; set; }

        /// <summary>
        /// 获得学位时间
        /// </summary>
        public string GetDegreeTime { get; set; }

        /// <summary>
        /// 学位授予单位
        /// </summary>
        public string GetDegreeCompany { get; set; }

        /// <summary>
        /// 毕业学校
        /// </summary>
        public string GraduationSchool { get; set; }

    }
}
