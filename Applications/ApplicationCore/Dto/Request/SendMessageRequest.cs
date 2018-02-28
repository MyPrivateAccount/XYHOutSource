using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Dto
{
    public class SendMessageRequest
    {
        public string MessageTypeCode { get; set; }

        public List<MessageItem> MessageList { get; set; }
    }
    public class MessageItem
    {
        //数据
        public List<TypeItem> MessageTypeItems { get; set; }

        //发送对象
        public List<string> UserIds { get; set; }
    }
    public class TypeItem
    {
        public string Key { get; set; }
        public string Value { get; set; }

    }
}
