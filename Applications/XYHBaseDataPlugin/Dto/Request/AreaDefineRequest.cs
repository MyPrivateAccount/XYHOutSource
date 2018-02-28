using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHBaseDataPlugin.Dto
{
    public class AreaDefineRequest
    {
        [StringLength(127)]
        public string Code { get; set; }
        [StringLength(255)]
        public string Level { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(255)]
        public string Abbreviation { get; set; }
        [StringLength(127)]
        public string ParentId { get; set; }
        public string Desc { get; set; }
        public int Order { get; set; }
    }
}
