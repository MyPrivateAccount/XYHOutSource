using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHContractPlugin.Dto.Request
{
  
        public class CompanyASearchCondition
        {
            /// <summary>
            /// 名称
            /// </summary>
            [StringLength(50, ErrorMessage = "KeyWord不能超过50个字符")]
            public string KeyWord { get; set; }
            /// <summary>
            /// 类型
            /// </summary>
            public string SearchType { get; set; }

            public bool OrderRule { get; set; }
            /// <summary>
            /// 项目地址
            /// </summary>
            public string Address { get; set; }

            public int pageIndex { get; set; }
            public int pageSize { get; set; }
        }
    
}
