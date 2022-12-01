using System.ComponentModel.DataAnnotations;

namespace mvc_andy.Models.com;



public class UserModel
{

    public int Id { get; set; }
    [Display(Name = "账号")]
    [StringLength(20, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 4)]
    public string UserName { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "密码")]
    [StringLength(20, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 4)]
    public string UserPass { get; set; } = string.Empty;

    [DataType(DataType.PhoneNumber)]
    [Display(Name = "手机号码")]
    public string Phone { get; set; } = string.Empty;

    [DataType(DataType.EmailAddress)]
    [Display(Name = "邮箱")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 0)]
    [Display(Name = "昵称")]
    public string NickName { get; set; } = string.Empty;

    [Display(Name = "角色")]
    public int RoleId { get; set; }

    [Display(Name = "状态")]
    public StatusType Status { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "创建日期")]
    public DateTime CreateAt { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "更新日期")]
    public DateTime UpdateAt { get; set; }

    public DeleteType Deleted { get; set; }


    public string StatusStr()
    {
        if (Status == StatusType.Enable)
        {
            return "启用";
        }
        return "禁用";
    }
}
