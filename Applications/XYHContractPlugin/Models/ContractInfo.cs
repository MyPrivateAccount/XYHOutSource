﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace XYHContractPlugin.Models
{
    public class ContractInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        [MaxLength(64)]
        public string Type { get; set; }
        [MaxLength(127)]
        public string Settleaccounts { get; set; }
        [MaxLength(127)]
        public string Commission { get; set; }
        public int? Relation { get; set; }//这个字段目前没用
        [MaxLength(127)]
        public string Name { get; set; }
        public string ContractEstate { get; set; }
        public int? Modify { get; set; }
        public string CurrentModify { get; set; }
        public int? Annex { get; set; }
        public int? Complement { get; set; }
        [MaxLength(127)]
        public string Follow { get; set; }
        [MaxLength(4000)]
        public string Remark { get; set; }
        [MaxLength(64)]
        public string ProjectName { get; set; }
        [MaxLength(64)]
        public string ProjectType { get; set; }
        [MaxLength(64)]
        public string CompanyA { get; set; }
        public string CompanyAT { get; set; }
        [MaxLength(127)]
        public string PrincipalpepoleA { get; set; }
        [MaxLength(127)]
        public string PrincipalpepoleB { get; set; }
        [MaxLength(127)]
        public string ProprincipalPepole { get; set; }
        [MaxLength(127)]
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }
        [MaxLength(127)]
        public string CreateDepartment { get; set; }
        [MaxLength(127)]
        public string CreateDepartmentID { get; set; }
        [MaxLength(127)]
        public string Organizate { get; set; }
        [MaxLength(127)]
        public string OrganizateID { get; set; }
        public bool IsDelete { get; set; }
        [MaxLength(127)]
        public string DeleteUser { get; set; }
        public DateTime? DeleteTime { get; set; }
        [MaxLength(64)]
        public string CommisionType { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Count { get; set; }
        public bool ReturnOrigin { get; set; }

        //[Key]
        [MaxLength(64)]
        public string Num { get; set; }
        public bool? IsFollow { get; set; }
        [MaxLength(127)]
        public string FollowId { get; set; }

        [MaxLength(256)]
        public string ProjectAddress { get; set; }
        [MaxLength(256)]
        public string CompanyAId { get; set; }
        [MaxLength(512)]
        public string OrganizateFullId { get; set; }
        public int? ExamineStatus { get; set; }//不作为数据库存储字段
        public DateTime? FollowTime { get; set; }
        [MaxLength(256)]
        public string Ext1 { get; set; }
        [MaxLength(256)]
        public string Ext2 { get; set; }
        /// <summary>
        /// 楼盘所属大区
        /// </summary>
        [MaxLength(255)]
        public string BuildingRegion { get; set; }
    }
}
