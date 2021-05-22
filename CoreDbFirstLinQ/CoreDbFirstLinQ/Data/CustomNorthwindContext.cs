using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CoreDbFirstLinQ.Controllers.HomeController;

namespace CoreDbFirstLinQ.Data
{
    public class CustomNorthwindContext : NorthwindContext
    {
        public DbSet<ProductModel> ProductModel { get; set; }

        public CustomNorthwindContext()
        {
                
        }

        public CustomNorthwindContext(DbContextOptions<NorthwindContext> options)
          : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductModel>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Name).HasColumnName("ProductName");
                entity.Property(e => e.Price).HasColumnName("UnitPrice");
            });
            

        }
    }
}
