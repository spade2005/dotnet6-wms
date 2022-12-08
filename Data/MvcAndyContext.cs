using Microsoft.EntityFrameworkCore;

using mvc_andy.Models.com;
using mvc_andy.Models.wms;

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

    public DbSet<GoodsCateModel> GoodsCateModels { get; set; }
    public DbSet<GoodsModel> GoodsModels { get; set; }
    public DbSet<OrderInModel> OrderInModels { get; set; }
    public DbSet<OrderOutModel> OrderOutModels { get; set; }
    public DbSet<OrderGoodsModel> OrderGoodsModels { get; set; }
    public DbSet<OrderAuditModel> OrderAuditModels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>().ToTable("com_user");
        modelBuilder.Entity<MenuModel>().ToTable("com_menu");
        modelBuilder.Entity<RoleMenuModel>().ToTable("com_role_menu");
        modelBuilder.Entity<RoleModel>().ToTable("com_role");


        modelBuilder.Entity<GoodsCateModel>().ToTable("wms_goods_cate");
        modelBuilder.Entity<GoodsModel>().ToTable("wms_goods");
        modelBuilder.Entity<OrderInModel>().ToTable("wms_order_in");
        modelBuilder.Entity<OrderOutModel>().ToTable("wms_order_out");
        modelBuilder.Entity<OrderGoodsModel>().ToTable("wms_order_goods");
        modelBuilder.Entity<OrderAuditModel>().ToTable("wms_order_audit");
    }
}