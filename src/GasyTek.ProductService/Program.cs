using GasyTek.ProductService.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core
builder.Services.AddDbContext<ProductsDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// Process to auto migration
var dbContext = new ProductsDbContext();
dbContext.Database.Migrate();

app.MapGet("/products", (ProductsDbContext dbContext) =>
{
    return dbContext.Products.ToListAsync();
})
.WithName("GetProductList");

app.Run();