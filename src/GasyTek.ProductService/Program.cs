using GasyTek.ProductService.Core;
using GasyTek.ProductService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using GasyTek.ProductService.Domain;
using OpenTelemetry.Logs;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core
builder.Services.AddDbContext<ProductsDbContext>();

// Redis
var redisConnection = ConnectionMultiplexer.Connect("localhost:6379");
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.InstanceName = "RedisCache";
    options.ConnectionMultiplexerFactory = () => Task.FromResult((IConnectionMultiplexer)redisConnection);
});

// OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
        tracerProviderBuilder
            .AddSource(DiagnosticsConfig.ActivitySource.Name)
            .AddNpgsql()
            .ConfigureResource(resource => resource.AddService(DiagnosticsConfig.ServiceName))
            .AddAspNetCoreInstrumentation((options) => options.Filter = httpContext =>
            {
                // only collect telemetry about all urls except /metrics
                return !httpContext.Request.Path.Equals("/metrics");
            })
            .AddRedisInstrumentation(redisConnection, options => options.SetVerboseDatabaseStatements = true)
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter())
    .WithMetrics(metricsProviderBuilder =>
        metricsProviderBuilder
            .ConfigureResource(resource => resource
                .AddService(DiagnosticsConfig.ServiceName))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter());

// Configure OpenTelemetry Logging
builder.Logging.ClearProviders();
builder.Logging.AddOpenTelemetry(opt =>
{
    var resourceBuilder = ResourceBuilder.CreateDefault();
    resourceBuilder.AddService(DiagnosticsConfig.ServiceName);
    opt.SetResourceBuilder(resourceBuilder);
    opt.AddOtlpExporter();
    opt.AddConsoleExporter();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// Process to auto migration
MigrateDatabase();

app.MapGet("/products", ([FromServices]ProductsDbContext dbContext, [FromServices]ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("ProductList");
    logger.Log(LogLevel.Information, "Get products list from ProductService");
    return dbContext.Products.ToListAsync();
});

app.MapGet("/products/{id:int}", object (
    [FromRoute] int id,
    [FromServices] ProductsDbContext dbContext,
    [FromServices] IDistributedCache cache,
    [FromServices]ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("Product");
    logger.Log(LogLevel.Information, "Get single product from ProductService");

    var cacheKey = $"Product:{id}";
    var product = cache.Get<Product>(cacheKey);
    if (product is null)
    {
        // Retrieve product from DB
        product = dbContext.Products.SingleOrDefault(it => it.Id == id);

        // Add product into cache
        cache.Set(cacheKey, product);

        return product is null ? Results.NotFound() : product;
    }
    else
    {
        return product;
    }
});

app.Run();

static void MigrateDatabase()
{
    var dbContext = new ProductsDbContext();
    dbContext.Database.Migrate();
}