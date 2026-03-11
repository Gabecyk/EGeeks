namespace StoreService.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using StoreService.Domain.Entities;

public class StoreDbContext : DbContext
{
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options)
    {
    }

    public DbSet<Store> Stores { get; set; }
    public DbSet<Product> Products { get; set; }
}