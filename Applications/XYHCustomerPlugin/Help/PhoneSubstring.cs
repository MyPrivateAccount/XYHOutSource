using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Help
{
    /// <summary>
    /// 电话截取类
    /// </summary>
    /// <returns></returns>
    public class PhoneSubstring
    {
        /// <summary>
        /// 电话截取
        /// </summary>
        /// <returns></returns>
        public string PhoneSubString(string phone)
        {
            if (phone.Length == 11)
            {
                var phone1 = phone.Substring(0, 3);
                var phone2 = "****";
                var phone3 = phone.Substring(phone.Length - 4, phone.Length);

                return phone1 + phone2 + phone3;
            }
            else
                return "";
        }
    }
}
