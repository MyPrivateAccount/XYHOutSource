using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Dto
{
    public class GetMyWorkResponse
    {
        public string UserId { get; set; }

        public string ContentId { get; set; }

        public string Type { get; set; }

        public string Content { get; set; }

        public int Sort { get; set; }

        public string Ext1 { get; set; }

        public string Ext2 { get; set; }

        public string Ext3 { get; set; }

        public string Ext4 { get; set; }


    }

    public class GetMyWorkTimesResponse
    {
        public DateTime DateTime { get; set; }

        public bool IsMeassage { get; set; }


    }
}
