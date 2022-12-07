using System.ComponentModel.DataAnnotations;
using mvc_andy.Models.com;

namespace mvc_andy.Models.wms;



public class OrderGoodsModel
{

    public int Id { get; set; }



    [Display(Name = "类型")]
    public OrderInOutType type { get; set; }

    [Display(Name = "所属订单")]
    public int OrderId { get; set; }

    [Display(Name = "订单商品")]
    public int GoodsId { get; set; }

    [Display(Name = "商品库存")]
    public int Quantity { get; set; } = 0;



    [DataType(DataType.DateTime)]
    [Display(Name = "创建日期")]
    public DateTime CreateAt { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "更新日期")]
    public DateTime UpdateAt { get; set; }
    public DeleteType Deleted { get; set; }

}
