using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Dto
{
    public class FeedBackSearchCondition
    {
        public string KeyWord { get; set; }
        public List<string> UserIds { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
