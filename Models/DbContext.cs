using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace PZApi.Models
{
    public class CarServiceConext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Part>()
                .Property(p => p.shipmentDate)
                .HasDefaultValueSql("now()");

            modelBuilder.Entity<Customer>()
                .Property(p => p.CreationDate)
                .HasDefaultValueSql("now()");
        }
        public CarServiceConext(DbContextOptions<CarServiceConext> options) :base(options) 
        {
        
        }

    }
}
