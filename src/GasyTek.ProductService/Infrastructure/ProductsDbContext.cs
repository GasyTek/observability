using GasyTek.ProductService.Domain;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace GasyTek.ProductService.Infrastructure
{
    public class ProductsDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=products;Username=postgres;Password=postgres");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed data
            var fileContent = File.ReadAllText("Data/products.json");
            var products = JsonConvert.DeserializeObject<Product[]>(fileContent);
            if (products is not null)
            {
                foreach (var product in products)
                {
                    modelBuilder.Entity<Product>().HasData(product);
                }
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
