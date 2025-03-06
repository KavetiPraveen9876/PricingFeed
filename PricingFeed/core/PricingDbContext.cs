
using Microsoft.EntityFrameworkCore;
using PricingFeed.Models;

public class PricingDbContext : DbContext
{
    public PricingDbContext(DbContextOptions<PricingDbContext> options) : base(options) { }
    public DbSet<PricingRecord> PricingRecords { get; set; }
}

