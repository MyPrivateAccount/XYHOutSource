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
        public bool IsSubmmitShop { get; set; }
        public bool IsSubmmitRelation { get; set; }
        public string Commission { get; set; }
        public int? Relation { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 是否续签
        /// </summary>
        public string Follow { get; set; }
        public string Remark { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public string CompanyA { get; set; }
        public int? CompanyAT { get; set; }
        public string PrincipalpepoleA { get; set; }
        public string PrincipalpepoleB { get; set; }
        public string ProprincipalPepole { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateDepartment { get; set; }
        public string Organizete { get; set; }
        public string CommisionType { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Count { get; set; }
        public int? ReturnOrigin { get; set; }
    }
    public class ContractContentResponse
    {
        /// <summary>
        /// 是否废弃
        /// </summary>
        public bool Discard { get; set; }
        public BaseInfoResponse BaseInfo { get; set; }
        public ContractEstateResponse EstateInfo { get; set; }
        public List<ContractAnnexResponse> AnnexInfo { get; set; }
        public List<ContractComplementResponse> ComplementInfo { get; set; }
        /// <summary>
        /// 是否有修改历史-包括new
        /// </summary>
        public List<ContractModifyResponse> Modifyinfo { get; set; }
        
    }
}
