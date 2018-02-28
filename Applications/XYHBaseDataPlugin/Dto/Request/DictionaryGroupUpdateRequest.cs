using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYHBaseDataPlugin.Models;

namespace XYHBaseDataPlugin.Dto
{
    public class DictionaryGroupUpdateRequest
    {
        [Required]
        [StringLength(127)]
        public string Id { get; set; }
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
            Id = dictionaryGroup.Id;
            Name = dictionaryGroup.Name;
            Desc = dictionaryGroup.Desc;
            Ext1Desc = dictionaryGroup.Ext1Desc;
            Ext2Desc = dictionaryGroup.Ext2Desc;
            ValueType = dictionaryGroup.ValueType;
        }

        public DictionaryGroup ToDataModel(string userId, DictionaryGroup dictionaryGroup)
        {
            if (dictionaryGroup == null)
            {
                return new DictionaryGroup();
            }
            if (!string.IsNullOrEmpty(Name))
            {
                dictionaryGroup.Name = Name;
            }
            if (!string.IsNullOrEmpty(Desc))
            {
                dictionaryGroup.Desc = Desc;
            }
            if (!string.IsNullOrEmpty(Ext1Desc))
            {
                dictionaryGroup.Ext1Desc = Ext1Desc;
                dictionaryGroup.HasExt1 = true;
            }
            if (!string.IsNullOrEmpty(Ext2Desc))
            {
                dictionaryGroup.Ext2Desc = Ext2Desc;
                dictionaryGroup.HasExt2 = true;
            }
            if (!string.IsNullOrEmpty(ValueType))
            {
                dictionaryGroup.ValueType = ValueType;
            }
            dictionaryGroup.UpdateTime = DateTime.Now;
            dictionaryGroup.UpdateUser = userId;
            return dictionaryGroup;
        }

    }
}
