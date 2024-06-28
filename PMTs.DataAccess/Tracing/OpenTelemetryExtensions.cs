using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;

namespace PMTs.DataAccess.Tracing
{
    public static class OpenTelemetryExtensions
    {
        public static void AddOpenTelemetryTracing(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OpenTelemetryParameters>(configuration.GetSection("OpenTelemetry"));
            //services.Configure<AspNetCoreInstrumentationOptions>(configuration.GetSection("AspNetCoreInstrumentation"));

            var openTelemetryParameters = configuration.GetSection("OpenTelemetry").Get<OpenTelemetryParameters>();

            ActivitySourceProvider.Source = new System.Diagnostics.ActivitySource(openTelemetryParameters!.ActivitySourceName!);
            Action<ResourceBuilder> configureResource = r => r.AddService(serviceName: openTelemetryParameters!.ServiceName!, serviceVersion: openTelemetryParameters!.ServiceVersion!, serviceInstanceId: openTelemetryParameters!.ServiceInstanceId!);
            services.AddOpenTelemetry()
                .WithTracing(builder =>
                {
                    builder.ConfigureResource(configureResource)
                    .AddSource(openTelemetryParameters!.ActivitySourceName!)
                    .SetSampler(new AlwaysOnSampler()).AddSqlClientInstrumentation(options => options.SetDbStatementForText = true)
                    .AddHttpClientInstrumentation(o =>
                    {
                        o.FilterHttpRequestMessage = (httpContext) => !httpContext.RequestUri.PathAndQuery.EndsWith("getScriptTag");

                        o.EnrichWithHttpRequestMessage = (activity, httpRequest) =>
                        {
                            activity.DisplayName = httpRequest.Method.Method + " " + httpRequest.RequestUri.PathAndQuery;
                        };
                    })
                    .AddAspNetCoreInstrumentation(o =>
                    {
                        o.Filter = (httpContext) => !string.IsNullOrEmpty(httpContext.Request.Path) 
                        && !(httpContext.Request.Path.Value.EndsWith(".js") ||
                        httpContext.Request.Path.Value.EndsWith(".css") ||
                        httpContext.Request.Path.Value.EndsWith(".map") ||
                        httpContext.Request.Path.Value.EndsWith(".jpg") ||
                        httpContext.Request.Path.Value.EndsWith(".jpeg") ||
                        httpContext.Request.Path.Value.EndsWith(".png"));
                        
                        // enrich activity with http request and response
                        var displayeName = string.Empty;
                        o.EnrichWithHttpRequest = (activity, httpRequest) =>
                        {
                            displayeName = httpRequest.Method + " " + httpRequest.Path.Value + httpRequest.QueryString;
                            activity.DisplayName = displayeName;
                            activity.SetTag("requestProtocol", httpRequest.Protocol);
                        };
                        o.EnrichWithHttpResponse = (activity, httpResponse) =>
                        {
                            activity.DisplayName = displayeName;
                            activity.SetTag("responseLength", httpResponse.ContentLength);
                        };

                        // automatically sets Activity Status to Error if an unhandled exception is thrown
                        o.RecordException = openTelemetryParameters.RecordException;
                        o.EnrichWithException = (activity, exception) =>
                        {
                            activity.SetTag("exceptionType", exception.GetType().ToString());
                            activity.SetTag("stackTrace", exception.StackTrace);
                        };
                    });
                    builder.AddOtlpExporter(otlp =>
                    {
                        otlp.Endpoint = new Uri("http://localhost:4317");
                    });
                });

        }

        public static void AddOpenTelemetryMetrics(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OpenTelemetryParameters>(configuration.GetSection("OpenTelemetry"));
            var openTelemetryParameters = configuration.GetSection("OpenTelemetry").Get<OpenTelemetryParameters>();

            services.AddOpenTelemetry().WithMetrics(options =>
            {
                options.AddMeter(openTelemetryParameters.ServiceName);
                options.AddAspNetCoreInstrumentation();
                options.AddHttpClientInstrumentation();
                options.AddRuntimeInstrumentation();
                options.AddProcessInstrumentation();
                options.AddPrometheusExporter();
                options.ConfigureResource(resource =>
                {
                    resource.AddService(serviceName: openTelemetryParameters.ServiceName,
                        serviceVersion: openTelemetryParameters.ServiceVersion);
                });
            });
        }
    }
}
