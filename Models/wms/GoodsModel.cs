using System.ComponentModel.DataAnnotations;
using mvc_andy.Models.com;

namespace mvc_andy.Models.wms;



public class GoodsModel
{

    public int Id { get; set; }
    [Display(Name = "标题")]
    [StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "子标题")]
    [StringLength(150, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 1)]
    public string SubTitle { get; set; } = string.Empty;

    [Display(Name = "商品分类")]
    public int CateId { get; set; } = 0;

    [Display(Name = "编号")]
    public string SerialNumber { get; set; } = string.Empty;

    [Display(Name = "sku")]
    public string Sku { get; set; } = string.Empty;

    [StringLength(250, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 0)]
    [Display(Name = "logo")]
    public string Logo { get; set; } = string.Empty;

    [Display(Name = "内容")]
    public string Content { get; set; } = string.Empty;

    [Display(Name = "是否上架")]
    public int IsOnSale { get; set; } = 0;

    [Display(Name = "排序")]
    public int SortBy { get; set; } = 0;

    [Display(Name = "当前库存")]
    public int Quantity { get; set; } = 0;

    [Display(Name = "总库存")]
    public int TotalQuantity { get; set; } = 0;

    [Display(Name = "唯一id")]
    public string UniqueId { get; set; } = string.Empty;

    public int TimeOfDay { get; set; } = 0;
    public int TimeOfMonth { get; set; } = 0;



    [DataType(DataType.DateTime)]
    [Display(Name = "创建日期")]
    public DateTime CreateAt { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "更新日期")]
    public DateTime UpdateAt { get; set; }
    public DeleteType Deleted { get; set; }

}
