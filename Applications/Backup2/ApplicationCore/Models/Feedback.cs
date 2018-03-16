using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationCore.Models
{
    public class Feedback
    {
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }
        [MaxLength(127)]
        public string UserId { get; set; }
        [MaxLength(512)]
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
