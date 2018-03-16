using ApplicationCore.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Dto
{
    public interface ITraceUpdate
    {
        UserInfo CreateUserInfo { get; set; }

        UserInfo UpdateUserInfo { get; set; }

        UserInfo DeleteUserInfo { get; set; }
    }
}
