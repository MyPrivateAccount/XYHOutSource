using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    /// <summary>
    /// 失效客户
    /// </summary>
    public class CustomerLoss
    {
        /// <summary>
        /// Id
        /// </summary>
        [MaxLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        [MaxLength(127)]
        public string CustomerId { get; set; }
        /// <summary>
        /// 失效类型（已购、失效、暂停、外泄）
        /// </summary>
        public LossType LossTypeId { get; set; }
        /// <summary>
        /// 失效备注
        /// </summary>
        [MaxLength(600)]
        public string LossRemark { get; set; }
        /// <summary>
        /// 失效部门
        /// </summary>
        [MaxLength(127)]
        public string LossDepartmentId { get; set; }
        /// <summary>
        /// 失效用户
        /// </summary>
        [MaxLength(127)]
        public string LossUserId { get; set; }
        /// <summary>
        /// 失效时间
        /// </summary>
        public DateTime LossTime { get; set; }
        /// <summary>
        /// 应该是求购的房子类型例如 商铺、住宅等等
        /// </summary>
        public NeedHouseType? HouseTypeId { get; set; }
        /// <summary>
        /// 删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 删除用户
        /// </summary>
        [MaxLength(127)]
        public string DeleteUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeleteTime { get; set; }

        [NotMapped]
        public CustomerInfo CustomerInfo { get; set; }

        [NotMapped]
        public Users LossUser { get; set; }
    }


    public enum LossType
    {
        Purchased = 1,
        Loss = 2,
        Pause = 3,
        Leak = 4
    }


}
