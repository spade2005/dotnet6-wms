using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using mvc_andy.Models.com;

namespace mvc_andy.Models.wms;


[Table("wms_order_goods")]
[Comment("单据商品明细")]
public class OrderGoodsModel
{

    [Column("id")]
    [Comment("the pk key auto")]
    public int Id { get; set; }



    [Column("type")]
    [Display(Name = "类型")]
    public OrderInOutType Type { get; set; }

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


    [NotMapped]
    public GoodsModel? goods { get; set; }

}
