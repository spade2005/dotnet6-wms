using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using mvc_andy.Models.com;

namespace mvc_andy.Models.wms;



public class GoodsModel
{
    public int Id { get; set; }
    [Display(Name = "标题")]
    [StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 1)]
    [Column(TypeName = "varchar(50)")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "子标题")]
    [StringLength(150, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 1)]
    [Column(TypeName = "varchar(150)")]
    public string SubTitle { get; set; } = string.Empty;

    [Display(Name = "商品分类")]
    public int CateId { get; set; } = 0;

    [Display(Name = "编号")]
    public string SerialNumber { get; set; } = string.Empty;

    [Display(Name = "sku")]
    public string Sku { get; set; } = string.Empty;

    [StringLength(250, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 0)]
    [Display(Name = "logo")]
    [JsonIgnore]
    public string Logo { get; set; } = string.Empty;

    [Display(Name = "内容")]
    public string Content { get; set; } = string.Empty;

    [Display(Name = "上架")]
    public GoodsSalesType IsOnSale { get; set; }

    [Display(Name = "排序")]
    [JsonIgnore]
    public int SortBy { get; set; } = 0;

    [Display(Name = "当前库存")]
    public int Quantity { get; set; } = 0;

    [Display(Name = "总库存")]
    public int TotalQuantity { get; set; } = 0;

    [Display(Name = "唯一id")]
    [JsonIgnore]
    public string UniqueId { get; set; } = string.Empty;

    [JsonIgnore]
    public int TimeOfDay { get; set; } = 0;

    [JsonIgnore]
    public int TimeOfMonth { get; set; } = 0;



    [DataType(DataType.DateTime)]
    [Display(Name = "创建日期")]
    [JsonIgnore]
    public DateTime CreateAt { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "更新日期")]
    [JsonIgnore]
    public DateTime UpdateAt { get; set; }

    [JsonIgnore]
    public DeleteType Deleted { get; set; }


    [NotMapped]
    public string CateName { get; set; } = string.Empty;
    public string IsOnSaleStr()
    {
        string str = string.Empty;
        switch (IsOnSale)
        {
            case GoodsSalesType.On:
                str = "已上架";
                break;
            case GoodsSalesType.Off:
                str = "已下架";
                break;
        }

        return str;
    }

}
