using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHBaseDataPlugin.Dto
{
    public class DictionaryDefineUpdateRequest
    {
        [Required]
        [StringLength(255)]
        public string Key { get; set; }
        public int Order { get; set; }
        [StringLength(255)]
        public string Ext1 { get; set; }
        [StringLength(255)]
        public string Ext2 { get; set; }


    }
}
