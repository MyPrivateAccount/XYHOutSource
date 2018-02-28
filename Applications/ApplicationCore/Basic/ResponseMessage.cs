using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore
{
    public class ResponseMessage
    {
        public string Code { get; set; }
        public string Message { get; set; }

        public ResponseMessage()
        {
            Code = ResponseCodeDefines.SuccessCode;
        }

        public bool IsSuccess()
        {
            if (Code == ResponseCodeDefines.SuccessCode)
            {
                return true;
            }
            return false;
        }
    }

    public class ResponseMessage<TEx> : ResponseMessage
    {
        public TEx Extension { get; set; }
    }

    public class PagingResponseMessage<Tentity> : ResponseMessage<List<Tentity>>
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public long TotalCount { get; set; }
    }
}
