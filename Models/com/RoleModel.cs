using System.ComponentModel.DataAnnotations;

namespace mvc_andy.Models.com;



public class RoleModel
{

    public int Id { get; set; }
    [Display(Name = "名称")]
    [StringLength(20, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 3)]
    public string? Name { get; set; }

    [Display(Name = "备注")]
    [StringLength(20, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 3)]
    public string? Mark { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "创建日期")]
    public DateTime CreateAt { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "更新日期")]
    public DateTime UpdateAt { get; set; }
    public DeleteType Deleted { get; set; }

}
