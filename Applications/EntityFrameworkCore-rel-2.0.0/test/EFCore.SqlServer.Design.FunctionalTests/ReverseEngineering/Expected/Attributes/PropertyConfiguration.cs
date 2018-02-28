using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2ETest.Namespace.SubDir
{
    public partial class PropertyConfiguration
    {
        [Column("PropertyConfigurationID")]
        public byte PropertyConfigurationId { get; set; }
        public DateTime WithDateDefaultExpression { get; set; }
        public DateTime WithDateFixedDefault { get; set; }
        public DateTime? WithDateNullDefault { get; set; }
        public Guid WithGuidDefaultExpression { get; set; }
        [StringLength(1)]
        public string WithVarcharNullDefaultValue { get; set; }
        public int WithDefaultValue { get; set; }
        public short? WithNullDefaultValue { get; set; }
        [Column(TypeName = "money")]
        public decimal WithMoneyDefaultValue { get; set; }
        public int A { get; set; }
        public int B { get; set; }
        [Column("SumOfAAndB")]
        public int? SumOfAandB { get; set; }
        [Required]
        public byte[] RowversionColumn { get; set; }
        [Column("PropertyConfiguration")]
        public int? PropertyConfiguration1 { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ComputedDateTimeColumn { get; set; }
    }
}
