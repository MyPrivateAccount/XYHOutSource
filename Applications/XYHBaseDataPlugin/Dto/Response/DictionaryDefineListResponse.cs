using System;
using System.Collections.Generic;
using System.Text;

namespace XYHBaseDataPlugin.Dto
{
    public class DictionaryDefineListResponse
    {
        public string GroupId { get; set; }
        public List<DictionaryDefineResponse> DictionaryDefines { get; set; }
    }
}
