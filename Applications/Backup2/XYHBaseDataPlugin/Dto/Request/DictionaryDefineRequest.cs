using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYHBaseDataPlugin.Models;

namespace XYHBaseDataPlugin.Dto
{
    public class DictionaryDefineRequest
    {
        [Required]
        [StringLength(127)]
        public string GroupId { get; set; }
        [Required]
        [StringLength(255)]
        public string Key { get; set; }
        [Required]
        [StringLength(255)]
        public string Value { get; set; }
        public int Order { get; set; }
        [StringLength(255)]
        public string Ext1 { get; set; }
        [StringLength(255)]
        public string Ext2 { get; set; }


        public DictionaryDefine ToDataModel(string userId)
        {
            DictionaryDefine dictionaryDefine = new DictionaryDefine();
            dictionaryDefine.GroupId = GroupId;
            dictionaryDefine.Key = Key;
            dictionaryDefine.Value = Value;
            dictionaryDefine.Ext1 = Ext1;
            dictionaryDefine.Ext2 = Ext2;
            dictionaryDefine.Order = Order;

            dictionaryDefine.CreateTime = DateTime.Now;
            dictionaryDefine.CreateUser = userId;
            dictionaryDefine.UpdateTime = DateTime.Now;
            dictionaryDefine.UpdateUser = userId;
            dictionaryDefine.IsDeleted = false;
            return dictionaryDefine;
        }


    }
}
