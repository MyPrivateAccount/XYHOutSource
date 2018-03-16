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
        public bool IsSubmmitShop { get; set; }
        public bool IsSubmmitRelation { get; set; }
        [MaxLength(127)]
        public string Commission { get; set; }
        public int? Relation { get; set; }
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
        public int? CompanyAT { get; set; }
        [MaxLength(32)]
        public string PrincipalpepoleA { get; set; }
        [MaxLength(32)]
        public string PrincipalpepoleB { get; set; }
        [MaxLength(32)]
        public string ProprincipalPepole { get; set; }
        [MaxLength(127)]
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }
        [MaxLength(32)]
        public string CreateDepartment { get; set; }
        public bool IsDelete { get; set; }
        [MaxLength(127)]
        public string DeleteUser { get; set; }
        public DateTime? DeleteTime { get; set; }
        [MaxLength(64)]
        public string CommisionType { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Count { get; set; }
        public int? ReturnOrigin { get; set; }
    }
}