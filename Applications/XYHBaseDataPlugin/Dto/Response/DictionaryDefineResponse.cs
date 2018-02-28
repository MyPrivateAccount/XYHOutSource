using System;
using System.Collections.Generic;
using System.Text;

namespace XYHBaseDataPlugin.Dto
{
    public class DictionaryDefineResponse
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
        public string Ext1 { get; set; }
        public string Ext2 { get; set; }
        public bool IsDeleted { get; set; }
        public string DeleteUser { get; set; }
        public DateTime? DeleteTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
