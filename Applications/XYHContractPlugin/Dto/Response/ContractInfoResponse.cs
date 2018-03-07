using System;
using System.Collections.Generic;
using System.Text;
using XYHContractPlugin.Dto.Response;

namespace XYHContractPlugin.Dto.Response
{
    public class ContractInfoResponse
    {
        public string ID { get; set; }
        public int? Number { get; set; }
        public int? Type { get; set; }
        public int? Relation { get; set; }
        /// <summary>
        /// 楼盘
        /// </summary>
        public ContractEstateResponse ContractEstate { get; set; }

        /// <summary>
        /// 是否有修改历史-包括new
        /// </summary>
        public int? Modifyed { get; set; }

        /// <summary>
        /// 是否上传附件
        /// </summary>
        public int? Annexed { get; set; }

        /// <summary>
        /// 是否补充附件内容
        /// </summary>
        public int? Complement { get; set; }

        /// <summary>
        /// 是否续签
        /// </summary>
        public string Follow { get; set; }
        public string Remark { get; set; }
        public string ProjectName { get; set; }
        public int? ProjectType { get; set; }
        public string CompanyA { get; set; }
        public string PrincipalpepoleA { get; set; }
        public string PrincipalpepoleB { get; set; }
        public string ProprincipalPepole { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateDepartment { get; set; }
        public int? CommisionType { get; set; }
        public DateTime? StarttTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Count { get; set; }
        public int? ReturnOrigin { get; set; }
    }
}
