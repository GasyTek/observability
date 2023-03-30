using GasyTek.ProductService.Core;
using GasyTek.ProductService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core
builder.Services.AddDbContext<ProductsDbContext>();

// OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
        tracerProviderBuilder
            .AddSource(DiagnosticsConfig.ActivitySource.Name)
            .AddNpgsql()
            .ConfigureResource(resource => resource.AddService(DiagnosticsConfig.ServiceName))
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter());
//.WithMetrics(metricsProviderBuilder =>
//    metricsProviderBuilder
//        .ConfigureResource(resource => resource
//            .AddService(DiagnosticsConfig.ServiceName))
//        .AddAspNetCoreInstrumentation()
//        .AddConsoleExporter());

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