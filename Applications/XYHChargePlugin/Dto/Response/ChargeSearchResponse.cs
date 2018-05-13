using ApplicationCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto.Response
{
    public class ChargeSearchResponse<T> : PagingResponseMessage<T>
    {
        public int ValidityContractCount { get; set; }
    }
}
