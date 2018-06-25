using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto
{
    public class ConfirmRequest
    {
        public string Id { get; set; }

        public int Status { get; set; }

        public string Message { get; set; }
    }
}
