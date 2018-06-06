using System;
using System.Collections.Generic;
using System.Text;

namespace XYHContractPlugin.Dto.Response
{
    public class BaseInfoResponse
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string Settleaccounts { get; set; }
        public bool IsSubmmitContractScan { get; set; }
        public bool IsSubmmitContractAnnex { get; set; }
        public bool IsSubmmitRelation { get; set; }
        public bool IsSubmmitNet { get; set; }
        public string Commission { get; set; }
        public int? Relation { get; set; }
        public string Name { get; set; }
        public string CurrentModify { get; set; }
        public int ExamineStatus { get; set; }
        /// <summary>
        /// 续签合同名
        /// </summary>
        public string Follow { get; set; }
        public string Remark { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public string CompanyA { get; set; }
        public string CompanyAT { get; set; }
        public string PrincipalpepoleA { get; set; }
        public string PrincipalpepoleB { get; set; }
        public string ProprincipalPepole { get; set; }
        public string CreateUser { get; set; }
        public string CreateUserName { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateDepartment { get; set; }
        public string CreateDepartmentID { get; set; }
        public string Organizate { get; set; }
        public string OrganizateID { get; set; }
        public string CommisionType { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Count { get; set; }
        public bool ReturnOrigin { get; set; }

        public string Num { get; set; }

        public bool? IsFollow { get; set; }
        public string FollowId { get; set; }
        public string ProjectAddress { get; set; }
        public string OrganizateFullId { get; set; }
        public string CompanyAId { get; set; }
        public DateTime? FollowTime { get; set; }
        public string Ext1 { get; set; }
        public string Ext2 { get; set; }
        public bool IsInvalid { get; set; }
    }
    public class ContractContentResponse
    {
        /// <summary>
        /// 是否废弃
        /// </summary>
        public bool Discard { get; set; }
        //public int AnnexGroupType { get; set; }//分组类型 合同扫描(a&0x1) 合同附件(a&0x2) 关系证明(a&0x8) 网签表(a&0x10)
        public BaseInfoResponse BaseInfo { get; set; }
        public ContractEstateResponse EstateInfo { get; set; }
        public List<ContractAnnexResponse> AnnexInfo { get; set; }
        public List<ContractComplementResponse> ComplementInfo { get; set; }
        /// <summary>
        /// 是否有修改历史-包括new
        /// </summary>
        public List<ContractModifyResponse> Modifyinfo { get; set; }
        public List<FileItemResponse> FileList { get; set; }

        public List<ContractInfoResponse> FollowHistory { get; set; }
    }
}
