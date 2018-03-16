using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHBaseDataPlugin.Dto
{
    public class DictionaryDefineDeleteRequest
    {
        [Required]
        [StringLength(127)]
        public string GroupId { get; set; }
        [Required]
        [StringLength(255)]
        public string Value { get; set; }
    }
}
