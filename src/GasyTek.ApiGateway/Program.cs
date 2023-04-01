using GasyTek.ApiGateway.Core;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddHttpClient<ProductApiClient>()
    .UseHttpClientMetrics();

// OpenTelemetry
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
            .AddConsoleExporter()
            .AddOtlpExporter())
    .WithMetrics(metricsProviderBuilder =>
        metricsProviderBuilder
            .ConfigureResource(resource => resource
                .AddService(DiagnosticsConfig.ServiceName))
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

/*
 * NOTE
 * Prometheus agent is enabled in addition to OpenTelemetry metrics because as of today,
 * OpenTelemetry metrics do not include Asp.Net / CLR metrics to prometheus. 
 * OpenTelemetry currently just expose one metric which is http duration but for sure, more will be added in the future.
 */
app.MapMetrics();
app.MapControllers();

app.Run();
