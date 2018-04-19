using ApplicationCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Dto.Response
{

    public class HumanSearchResponse<T> : PagingResponseMessage<T>
    {
        public int ValidityContractCount { get; set; }
    }
}
