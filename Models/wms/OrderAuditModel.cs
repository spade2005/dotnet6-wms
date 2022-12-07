using System.ComponentModel.DataAnnotations;
using mvc_andy.Models.com;

namespace mvc_andy.Models.wms;



public class OrderAuditModel
{

    public int Id { get; set; }

    [Display(Name = "类型")]
    public OrderInOutType type { get; set; }

    [Display(Name = "所属订单")]
    public int OrderId { get; set; }

    [Display(Name = "操作人")]
    public int UserId { get; set; }

    [Display(Name = "操作人")]
    public string UserName { get; set; } = string.Empty;

    [Display(Name = "状态")]
    public OrderStatusType status { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "创建日期")]
    public DateTime CreateAt { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "更新日期")]
    public DateTime UpdateAt { get; set; }
    public DeleteType Deleted { get; set; }

}
