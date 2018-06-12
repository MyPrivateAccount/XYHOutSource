using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


//localhost:5000/api/user   POST
public class UserInfoRequest
{
    [RegularExpression(@"^\d{11}$", ErrorMessage = "客户电话格式不正确")]
    public string PhoneNumber { get; set; }
    [StringLength(50)]
    public string Password { get; set; }
    [StringLength(50)]
    public string Email { get; set; }
    [RegularExpression(@"^\w+$", ErrorMessage = "用户名包含特殊字符")]
    public string UserName { get; set; }
    [StringLength(127)]
    public string OrganizationId { get; set; }
    [StringLength(127)]
    public string FilialeId { get; set; }
    [StringLength(256)]
    public string TrueName { get; set; }
    [StringLength(127)]
    public string Position { get; set; }
    [StringLength(512)]
    public string Avatar { get; set; }
} 