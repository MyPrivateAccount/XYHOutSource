using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYHCustomerPlugin.Dto.Request;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto
{
    public class CustomerInfoCreateRequest
    {
        [Key]
        [StringLength(127, ErrorMessage = "Id不能超过127个字符")]
        public string Id { get; set; }

        [StringLength(50, ErrorMessage = "CustomerName不能超过50个字符")]
        public string CustomerName { get; set; }

        [StringLength(255, ErrorMessage = "Source不能超过255个字符")]
        public string Source { get; set; }//来源 上门 报纸 网络 派单 端口   字典数据

        [StringLength(50, ErrorMessage = "MainPhone不能超过50个字符")]
        public string MainPhone { get; set; }//主用电话号码

        [StringLength(20, ErrorMessage = "QQ不能超过20个字符")]
        public string QQ { get; set; }

        [StringLength(20, ErrorMessage = "WeChat不能超过20个字符")]
        public string WeChat { get; set; }

        [StringLength(50, ErrorMessage = "Email不能超过50个字符")]
        public string Email { get; set; }

        public bool Sex { get; set; }

        public DateTime? Birthday { get; set; }
        
        public DateTime? ConvenientHouseTime { get; set; }//方便看房时间   字典数据

        public CustomerStatus? CustomerStatus { get; set; }//客户状态  字典数据

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(1000, ErrorMessage = "Mark不能超过1000个字符")]
        public string Mark { get; set; }

        public CustomerDemandRequest CustomerDemandRequest { get; set; }

        public List<RelationHouseRequest> HousingResourcesRequest { get; set; }

        public List<CustomerPhoneRequest> Phones { get; set; }
    }
}
