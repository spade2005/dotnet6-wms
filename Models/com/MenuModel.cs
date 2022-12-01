using System.ComponentModel.DataAnnotations;

namespace mvc_andy.Models.com;



public class MenuModel
{
    public int Id { get; set; }
    [Display(Name = "名称")]
    [StringLength(20, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;
    [Display(Name = "级别")]
    public int ParentId { get; set; }
    [Display(Name = "类型")]
    public AuthType Type { get; set; }
    [Display(Name = "排序")]
    public int SortBy { get; set; }
    [Display(Name = "样式")]
    public string Style { get; set; } = string.Empty;
    [Display(Name = "uri")]
    public string Router { get; set; } = string.Empty;
    [Display(Name = "备注")]
    public string Mark { get; set; } = string.Empty;

    [Display(Name = "状态")]
    public StatusType Status { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "创建日期")]
    public DateTime CreateAt { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "更新日期")]
    public DateTime UpdateAt { get; set; }
    public DeleteType Deleted { get; set; }

}
