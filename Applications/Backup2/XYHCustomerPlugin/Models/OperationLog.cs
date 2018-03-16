using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    /// <summary>
    /// 操作日志
    /// </summary>
    public class OperationLog
    {
        /// <summary>
        /// Id
        /// </summary>
        [MaxLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 操作类型 OperateType(查看、添加、修改、删除)
        /// </summary>
        public int OperateType { get; set; }

        /// <summary>
        /// 操作用户
        /// </summary>
        [MaxLength(127)]
        public string UserId { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [MaxLength(256)]
        public string TrueName { get; set; }

        /// <summary>
        /// 日志等级 
        /// </summary>
        public string LogLevel { get; set; }
        /// <summary>
        /// 请求地址
        /// </summary>
        [MaxLength(512)]
        public string LogAddress { get; set; }
        /// <summary>
        /// 日志内容
        /// </summary>
        [MaxLength(1024)]
        public string LogMessage { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatTime { get; set; }
        /// <summary>
        /// 关联资源的Id
        /// </summary>
        [MaxLength(127)]
        public string RelationId { get; set; }
    }


    public enum OperateType
    {
        Index = 1,
        Create = 2,
        Update = 4,
        Delete = 8
    }

}
