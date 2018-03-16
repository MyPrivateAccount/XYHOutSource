using ExamineCenterPlugin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExamineCenterPlugin.Dto
{
    public class UserExamineNoticeListConditon
    {
        public List<NoticeStatus> Status { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }


    }

}
