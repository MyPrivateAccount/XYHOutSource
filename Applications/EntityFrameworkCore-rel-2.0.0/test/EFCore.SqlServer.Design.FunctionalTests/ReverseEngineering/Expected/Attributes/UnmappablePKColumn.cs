using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2ETest.Namespace.SubDir
{
    [Table("UnmappablePKColumn")]
    public partial class UnmappablePkcolumn
    {
        [Column("UnmappablePKColumnID")]
        public int UnmappablePkcolumnId { get; set; }
        [Required]
        [Column("AColumn")]
        [StringLength(20)]
        public string Acolumn { get; set; }
        public int ValueGeneratedOnAddColumn { get; set; }
    }
}
