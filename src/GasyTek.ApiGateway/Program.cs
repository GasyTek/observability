using GasyTek.ApiGateway.Core;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddHttpClient<ProductApiClient>();

// Configure OpenTelemetry Tracing & Metrics
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
        tracerProviderBuilder
            .AddSource(DiagnosticsConfig.ActivitySource.Name)
            .ConfigureResource(resource => resource.AddService(DiagnosticsConfig.ServiceName))
            .AddAspNetCoreInstrumentation((options) => options.Filter = httpContext =>
            {
                // only collect telemetry about all urls except /metrics
                return !httpContext.Request.Path.Equals("/metrics");
            })
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter())
    .WithMetrics(metricsProviderBuilder =>
        metricsProviderBuilder
            .ConfigureResource(resource => resource.AddService(DiagnosticsConfig.ServiceName))
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

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();