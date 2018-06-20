using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHChargePlugin.Dto
{
    public class FileInfoResponse
    {
        public string FileGuid { get; set; }
        public string Name { get; set; }

      
        public string Type { get; set; }

        
        public string FileExt { get; set; }
        public double Size { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        
        public string Uri { get; set; }
        public string Summary { get; set; }
       
        public string Ext1 { get; set; }
      
        public string Ext2 { get; set; }

        public bool IsDeleted { get; set; }

        
        public string Driver { get; set; }
      
        public string Group { get; set; }

        public string Url { get; set; }
    }
}
