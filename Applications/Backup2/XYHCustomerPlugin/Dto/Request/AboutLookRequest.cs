using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto.Request
{
    public class AboutLookRequest
    {
        /// <summary>
        /// Id
        /// </summary>
        [StringLength(127, ErrorMessage = "Id最大字符为127")]
        public string Id { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        [StringLength(127, ErrorMessage = "CustomerId最大字符为127")]
        public string CustomerId { get; set; }
        /// <summary>
        /// 客户编号
        /// </summary>
        [StringLength(50, ErrorMessage = "CustomerNo最大字符为50")]
        public string CustomerNo { get; set; }
        /// <summary>
        /// 客户姓名
        /// </summary>
        [StringLength(50, ErrorMessage = "CustomerName最大字符为50")]
        public string CustomerName { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        [StringLength(127, ErrorMessage = "UserId最大字符为127")]
        public string UserId { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        [StringLength(127, ErrorMessage = "DepartmentId最大字符为127")]
        public string DepartmentId { get; set; }
        /// <summary>
        /// 约看时间
        /// </summary>
        public DateTime AboutTime { get; set; }
        /// <summary>
        /// 约看状态(待带看，已取消，已带看，已过期)
        /// </summary>
        public AboutLookState AboutState { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(1000, ErrorMessage = "Remark最大字符为1000")]
        public string Remark { get; set; }
        /// <summary>
        /// 约看房源(多个逗号隔开)
        /// </summary>
        [StringLength(1000, ErrorMessage = "AboutHouse最大字符为1000")]
        public string AboutHouse { get; set; }
        /// <summary>
        /// 房源类型（楼盘、商铺、住宅等等）
        /// </summary>
        public NeedHouseType? HouseTypeId { get; set; }
    }



    /// <summary>
    /// 我的带看查询请求
    /// </summary>
    public class MyAboutLookCondition
    {
        public int pageIndex { get; set; }

        public int pageSize { get; set; }

        /// <summary>
        /// 0：全部客户 1：报备 2：待成交 3：已成交 4：已失效
        /// </summary>
        public int mark { get; set; }
    }
}
