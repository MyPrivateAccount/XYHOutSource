using System.ComponentModel.DataAnnotations;

namespace XYHCustomerPlugin.Dto.Request
{
    public class CustomerPhoneRequest
    {
        /// <summary>
        /// 电话
        /// </summary>
        [StringLength(127)]
        public string Phone { get; set; }
        /// <summary>
        /// 是否主要电话
        /// </summary>
        public bool IsMain { get; set; }
    }
}
