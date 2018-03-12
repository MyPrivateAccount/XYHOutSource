using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Dto.Response
{
    public class DealFileInfoResponse
    {
        public string FileGuid { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string FileExt { get; set; }
        public double Size { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Uri { get; set; }
        public string Ext1 { get; set; }
        public string Ext2 { get; set; }

        public string ProofType { get; set; }
    }
}
