using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYHBaseDataPlugin.Models;

namespace XYHBaseDataPlugin.Dto
{
    public class DictionaryGroupCreateRequest
    {
        [Required]
        [StringLength(127)]
        public string Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(255)]
        public string Desc { get; set; }
        [StringLength(127)]
        public string ValueType { get; set; }
        [MaxLength(255)]
        public string Ext1Desc { get; set; }
        [MaxLength(255)]
        public string Ext2Desc { get; set; }


        public void CopyFrom(DictionaryGroup dictionaryGroup)
        {
            Name = dictionaryGroup.Name;
            Desc = dictionaryGroup.Desc;
            Ext1Desc = dictionaryGroup.Ext1Desc;
            Ext2Desc = dictionaryGroup.Ext2Desc;
            ValueType = dictionaryGroup.ValueType;
        }

        public DictionaryGroup ToDataModel(string userId)
        {
            DictionaryGroup dictionaryGroup = new DictionaryGroup();
            dictionaryGroup.Id = Id;
            dictionaryGroup.Name = Name;
            dictionaryGroup.Desc = Desc;
            dictionaryGroup.Ext1Desc = Ext1Desc;
            dictionaryGroup.Ext2Desc = Ext2Desc;
            dictionaryGroup.ValueType = ValueType;
            dictionaryGroup.HasExt1 = !string.IsNullOrEmpty(Ext1Desc);
            dictionaryGroup.HasExt2 = !string.IsNullOrEmpty(Ext2Desc);
            dictionaryGroup.CreateTime = DateTime.Now;
            dictionaryGroup.CreateUser = userId;
            dictionaryGroup.UpdateTime = DateTime.Now;
            dictionaryGroup.UpdateUser = userId;
            dictionaryGroup.IsDeleted = false;
            return dictionaryGroup;
        }


    }
}
