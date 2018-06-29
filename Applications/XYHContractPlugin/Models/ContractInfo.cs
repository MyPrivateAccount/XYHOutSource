using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace XYHContractPlugin.Models
{
    /// <summary>
    /// 合同信息表
    /// </summary>
    public class ContractInfo
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(64)]
        public string Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(127)]
        public string Settleaccounts { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(127)]
        public string Commission { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? Relation { get; set; }//这个字段目前没用
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(127)]
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ContractEstate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? Modify { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CurrentModify { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? Annex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? Complement { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(127)]
        public string Follow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(4000)]
        public string Remark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(64)]
        public string ProjectName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(64)]
        public string ProjectType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(64)]
        public string CompanyA { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CompanyAT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(127)]
        public string PrincipalpepoleA { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(127)]
        public string PrincipalpepoleB { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(127)]
        public string ProprincipalPepole { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(127)]
        public string CreateUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(127)]
        public string CreateDepartment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(127)]
        public string CreateDepartmentID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(127)]
        public string Organizate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(127)]
        public string OrganizateID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(127)]
        public string DeleteUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? DeleteTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(64)]
        public string CommisionType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? Count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool ReturnOrigin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //[Key]
        [MaxLength(64)]
        public string Num { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? IsFollow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(127)]
        public string FollowId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(256)]
        public string ProjectAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(256)]
        public string CompanyAId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(512)]
        public string OrganizateFullId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? ExamineStatus { get; set; }//不作为数据库存储字段
        /// <summary>
        /// 
        /// </summary>
        public DateTime? FollowTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(256)]
        public string Ext1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(256)]
        public string Ext2 { get; set; }

        /// <summary>
        /// 楼盘所属大区
        /// </summary>
        [MaxLength(255)]
        public string BuildingRegion { get; set; }
    }
}
