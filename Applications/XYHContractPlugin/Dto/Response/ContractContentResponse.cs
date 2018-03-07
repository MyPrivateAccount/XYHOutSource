using System;
using System.Collections.Generic;
using System.Text;

namespace XYHContractPlugin.Dto.Response
{
    public class ContractContentResponse
    {
        public string ProName { get; set; }
        public int? ProType { get; set; }
        public string CompanyNameA { get; set; }
        public string PrincipalNameA { get; set; }
        public string PrincipalNameB { get; set; }
        public string PrincipalNameAll { get; set; }
        public string CommitName { get; set; }
        public DateTime? CommitTime { get; set; }
        public string CommitDepartment { get; set; }
        public int? CommitSiontype { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Count { get; set; }
        public int? ReturnOrigin { get; set; }
    }
}
