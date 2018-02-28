using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Dto;

namespace ApplicationCore.Dto
{
    public class TraceUpdateBase : ITraceUpdate
    {
        public UserInfo CreateUserInfo { get; set; }
        public UserInfo UpdateUserInfo { get; set; }
        public UserInfo DeleteUserInfo { get; set; }
    }
}
