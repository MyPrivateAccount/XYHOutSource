using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Models
{
    /// <summary>
    /// 教育信息
    /// </summary>
    public class HumanEducationInfo
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }

        /// <summary>
        /// 人事Id
        /// </summary>
        [Required]
        [MaxLength(127)]
        public string HumanId { get; set; }

        /// <summary>
        /// 学历
        /// </summary>
        [MaxLength(255)]
        public string Education { get; set; }

        /// <summary>
        /// 所学专业
        /// </summary>
        [MaxLength(255)]
        public string Major { get; set; }

        /// <summary>
        /// 学习形式
        /// </summary>
        [MaxLength(255)]
        public string LearningType { get; set; }

        /// <summary>
        /// 毕业证书
        /// </summary>
        [MaxLength(255)]
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
        [MaxLength(255)]
        public string GetDegree { get; set; }

        /// <summary>
        /// 获得学位时间
        /// </summary>
        public DateTime? GetDegreeTime { get; set; }

        /// <summary>
        /// 学位授予单位
        /// </summary>
        [MaxLength(255)]
        public string GetDegreeCompany { get; set; }

        /// <summary>
        /// 毕业学校
        /// </summary>
        [MaxLength(255)]
        public string GraduationSchool { get; set; }

        [MaxLength(127)]
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }
        [MaxLength(127)]
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(127)]
        public string DeleteUser { get; set; }
        public DateTime? DeleteTime { get; set; }
    }
}
