using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Models
{
    public class DictionaryDefine
    {
        public string GroupId { get; set; }
       
        public string Key { get; set; }
       
        public string Value { get; set; }
        public int Order { get; set; }
       
        public string Ext1 { get; set; }
       
        public string Ext2 { get; set; }
        public bool IsDeleted { get; set; }
     

    }
}
