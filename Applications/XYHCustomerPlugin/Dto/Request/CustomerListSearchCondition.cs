using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto.Request
{
    /// <summary>
    /// 查询请求类
    /// </summary>
    public class CustomerListSearchCondition
    {
        /// <summary>
        /// 用户条件
        /// </summary>
        [StringLength(50, ErrorMessage = "KeyWord不能超过50个字符")]
        public string KeyWord { get; set; }

        /// <summary>
        /// 0:客户 1:业务员
        /// </summary>
        public int SearchType { get; set; }

        /// <summary>
        /// 重要程度
        /// </summary>
        public Importance? Importance { get; set; }

        /// <summary>
        /// 需求等级
        /// </summary>
        public DemandLevel? DemandLevel { get; set; }

        /// <summary>
        /// 0：已报备 1：已带看
        /// </summary>
        public List<int> BusinessStage { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public List<string> Source { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public string ProvinceId { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        public string CityId { get; set; }

        /// <summary>
        /// 区域Id，区域表Id
        /// </summary>
        public string AreaId { get; set; }

        /// <summary>
        /// 价格区间开始
        /// </summary>
        public decimal? PriceStart { get; set; }

        /// <summary>
        /// 价格区间结束
        /// </summary>
        public decimal? PriceEnd { get; set; }

        /// <summary>
        /// 面积区间开始
        /// </summary>
        public decimal? AcreageStart { get; set; }

        /// <summary>
        /// 面积区间结束
        /// </summary>
        public decimal? AcreageEnd { get; set; }

        /// <summary>
        /// 创建时间开始
        /// </summary>
        public DateTime? CreateDateStart { get; set; }

        /// <summary>
        /// 创建时间结束
        /// </summary>
        public DateTime? CreateDateEnd { get; set; }

        /// <summary>
        /// 跟进时间开始
        /// </summary>
        public DateTime? FollowUpStart { get; set; }
        
        /// <summary>
        /// 跟进时间结束
        /// </summary>
        public DateTime? FollowUpEnd { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        [StringLength(50, ErrorMessage = "DepartmentId不能超过50个字符")]
        public string DepartmentId { get; set; }

        /// <summary>
        /// 是否显示完整的电话号码
        /// </summary>
        //public bool IsCompletenessPhone { get; set; }

        /// <summary>
        /// true:由多到少 false:由少到多
        /// </summary>
        public bool OrderRule { get; set; }

        /// <summary>
        /// 是否只显示重复客户
        /// </summary>
        public bool IsOnlyRepeat { get; set; }

        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }
}
