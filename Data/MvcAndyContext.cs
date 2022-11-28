using Microsoft.EntityFrameworkCore;

using mvc_andy.Models.com;

namespace mvc_andy.Data;

public class MvcAndyContext : DbContext
{
    public MvcAndyContext(DbContextOptions<MvcAndyContext> options)
        : base(options)
    {
    }

    public DbSet<UserModel> UserModels { get; set; }
    public DbSet<RoleModel> RoleModels { get; set; }
    public DbSet<RoleMenuModel> RoleMenuModels { get; set; }
    public DbSet<MenuModel> MenuModels { get; set; }
}