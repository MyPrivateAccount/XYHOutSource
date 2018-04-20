using ApplicationCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace XYHContractPlugin.Dto.Response
{
    public class CompanyASearchResponse<T> : PagingResponseMessage<T>
    {
        public int ValidityContractCount { get; set; }
    }
}
