using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ApplicationCore.Dto
{
   public class FeedbackRequest
    {
        [MaxLength(512)]
        public string Content { get; set; }
    }
}
