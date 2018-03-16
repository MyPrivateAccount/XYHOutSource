using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto.Request
{
    public class CustomerDemandRequest
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [StringLength(127, ErrorMessage = "Id最大长度不超过127")]
        public string Id { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        [StringLength(127, ErrorMessage = "CustomerId最大长度不超过127")]
        public string CustomerId { get; set; }
        /// <summary>
        /// 客户类型（应该是已成交已失效等类型TODO）
        /// </summary>
        public int TypeId { get; set; }

        public string RequirementType { get; set; }

        /// <summary>
        /// 区域Id
        /// </summary>
        [StringLength(255, ErrorMessage = "AreaFullName最大长度不超过255")]
        public string AreaFullName { get; set; }

        /// <summary>
        /// 区域Id
        /// </summary>
        [StringLength(127, ErrorMessage = "AreaId最大长度不超过127")]
        public string AreaId { get; set; }

        public string DistrictId { get; set; }

        /// <summary>
        /// 重要程度（重要、一般、无价值）
        /// </summary>
        public Importance? Importance { get; set; }

        /// <summary>
        /// 需求等级（A,B,C,D）
        /// </summary>
        public DemandLevel? DemandLevel { get; set; }

        /// <summary>
        /// 面积区间开始
        /// </summary>
        public decimal? AcreageStart { get; set; }
        /// <summary>
        /// 面积区间结束
        /// </summary>
        public decimal? AcreageEnd { get; set; }
        /// <summary>
        /// 价格区间开始
        /// </summary>
        public decimal? PriceStart { get; set; }
        /// <summary>
        /// 价格区间结束
        /// </summary>
        public decimal? PriceEnd { get; set; }
        /// <summary>
        /// 购房方式  1商贷  3全款
        /// </summary>
        [StringLength(127, ErrorMessage = "AreaId最大长度不超过127")]
        public string PurchaseWay { get; set; }
        /// <summary>
        /// 购买动机,多选逗号分隔（投资，开公司，做生意，开餐馆）
        /// </summary>
        [StringLength(1000, ErrorMessage = "PurchaseMotivation最大长度不超过1000")]
        public string PurchaseMotivation { get; set; }

        /// <summary>
        /// 省内ID
        /// </summary>
        [StringLength(127, ErrorMessage = "ProvinceId最大长度不超过127")]
        public string ProvinceId { get; set; }

        /// <summary>
        /// 市内ID
        /// </summary>
        [StringLength(127, ErrorMessage = "ProvinceId最大长度不超过127")]
        public string CityId { get; set; }


    }
}
