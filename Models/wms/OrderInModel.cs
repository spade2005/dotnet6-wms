using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mvc_andy.Models.com;

namespace mvc_andy.Models.wms;



public class OrderInModel
{

    public int Id { get; set; }

    [Display(Name = "订单号")]
    [StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 1)]
    [Required]
    public string OrderNo { get; set; } = string.Empty;

    [Display(Name = "备注")]
    [StringLength(250, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 1)]
    public string Mark { get; set; } = string.Empty;


    [Display(Name = "入库商品")]
    public int GoodsNum { get; set; } = 0;

    [Display(Name = "单据状态")]
    public OrderStatusType OrderStatus { get; set; }

    [Display(Name = "库存状态")]
    public StockType StockStatus { get; set; }


    [DataType(DataType.DateTime)]
    [Display(Name = "审核日期")]
    public DateTime AuditTime { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "库存日期")]
    public DateTime StockTime { get; set; }


    public int TimeOfDay { get; set; } = 0;
    public int TimeOfMonth { get; set; } = 0;

    [DataType(DataType.DateTime)]
    [Display(Name = "创建日期")]
    public DateTime CreateAt { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "更新日期")]
    public DateTime UpdateAt { get; set; }
    public DeleteType Deleted { get; set; }

    [NotMapped]
    public List<OrderGoodsModel>? GoodsList { get; set; }

    public string OrderStatusStr()
    {
        string str = string.Empty;
        switch (OrderStatus)
        {
            case OrderStatusType.Pending:
                str = "待审核";
                break;
            case OrderStatusType.Success:
                str = "审核成功";
                break;
            case OrderStatusType.Failed:
                str = "审核失败";
                break;
        }
        return str;
    }

    public string StockStatusStr()
    {
        string str = string.Empty;
        switch (StockStatus)
        {
            case StockType.Pending:
                str = "待处理";
                break;
            case StockType.Success:
                str = "处理成功";
                break;
            case StockType.Default:
                str = "等待审核";
                break;
        }
        return str;

    }

}
