
namespace mvc_andy.Models.com;



public class RoleMenuModel
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public int MenuId { get; set; }
    public DeleteType Deleted { get; set; }

}
