using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2ETest.Namespace.SubDir
{
    public partial class OneToOneDependent
    {
        [Column("OneToOneDependentID1")]
        public int OneToOneDependentId1 { get; set; }
        [Column("OneToOneDependentID2")]
        public int OneToOneDependentId2 { get; set; }
        [Required]
        [StringLength(20)]
        public string SomeDependentEndColumn { get; set; }

        [ForeignKey("OneToOneDependentId1,OneToOneDependentId2")]
        [InverseProperty("OneToOneDependent")]
        public OneToOnePrincipal OneToOneDependentNavigation { get; set; }
    }
}
