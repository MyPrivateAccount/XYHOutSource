using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace XYHCustomerPlugin.Help
{
    //AES 加密解密帮助类
    public class EncDncHelper
    {
        /// <summary>    
        /// AES支持128（16字节） 196（24字节） 256（32字节）位加密    
        /// </summary>    
        private string mykey = "xinyaohangW1i2L0I2o2B7L9G7n5c7X5";


        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="toEncrypt">明文</param>
        /// <returns>密文</returns>
        public string Encrypt(string toEncrypt)
        {
            if (string.IsNullOrEmpty(toEncrypt)) return toEncrypt;

            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(mykey);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="toDecrypt">密文</param>
        /// <returns>明文</returns>
        public string Decrypt(string toDecrypt)
        {
            var res = string.Empty;
            if (string.IsNullOrEmpty(toDecrypt))
            {
                return toDecrypt;
            }
            try
            {
                byte[] keyArray = UTF8Encoding.UTF8.GetBytes(mykey);
                byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                res = UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                res = toDecrypt;
            }


            return res;
        }

    }
}
