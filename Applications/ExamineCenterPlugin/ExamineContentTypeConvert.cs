using System;
using System.Collections.Generic;
using System.Text;

namespace ExamineCenterPlugin
{
    public class ExamineContentTypeConvert
    {
        public static string GetContentTypeString(string type)
        {
            switch (type)
            {
                case "building":
                    return "新增楼盘";
                case "shops":
                    return "新增商铺";
                case "ShopsAdd":
                    return "商铺加推";
                case "ReportRule":
                    return "报备规则";
                case "CommissionType":
                    return "佣金方案";
                case "BuildingNo":
                    return "楼栋批次";
                case "DiscountPolicy":
                    return "优惠政策";
                case "Image":
                    return "图片";
                case "Attachment":
                    return "附件";
                case "Price":
                    return "价格";
                case "TransferCustomer":
                    return "调客审核";
                default:
                    return "";
            }
            
        }
    }
}

