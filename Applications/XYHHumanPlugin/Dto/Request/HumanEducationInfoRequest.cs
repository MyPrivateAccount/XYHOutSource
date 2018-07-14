using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Dto.Request
{
    public class HumanEducationInfoRequest
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        [StringLength(127)]
        public string Id { get; set; }

        /// <summary>
        /// 人事Id
        /// </summary>
        [StringLength(127)]
        public string HumanId { get; set; }

        /// <summary>
        /// 学历
        /// </summary>
        [StringLength(255)]
        public string Education { get; set; }

        /// <summary>
        /// 所学专业
        /// </summary>
        [StringLength(255)]
        public string Major { get; set; }

        /// <summary>
        /// 学习形式
        /// </summary>
        [StringLength(255)]
        public string LearningType { get; set; }

        /// <summary>
        /// 毕业证书
        /// </summary>
        [StringLength(255)]
        public string GraduationCertificate { get; set; }

        /// <summary>
        /// 入学时间
        /// </summary>
        public DateTime? EnrolmentTime { get; set; }

        /// <summary>
        /// 毕业时间
        /// </summary>
        public DateTime? GraduationTime { get; set; }

        /// <summary>
        /// 获得学位
        /// </summary>
        [StringLength(255)]
        public string GetDegree { get; set; }

        /// <summary>
        /// 获得学位时间
        /// </summary>
        public DateTime? GetDegreeTime { get; set; }

        /// <summary>
        /// 学位授予单位
        /// </summary>
        [StringLength(255)]
        public string GetDegreeCompany { get; set; }

        /// <summary>
        /// 毕业学校
        /// </summary>
        [StringLength(255)]
        public string GraduationSchool { get; set; }

    }
}
