using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logging
{
    public static class Logging
    {
        public static Action<HostBuilderContext, LoggerConfiguration> cfg => (context, loggingConfiguration) =>
        {
            var env = context.HostingEnvironment;
            loggingConfiguration.MinimumLevel.Information()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", env.ApplicationName)
            .Enrich.WithProperty("EnviorenmentName", env.EnvironmentName)
            .Enrich.WithExceptionDetails()
            .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Warning)
            .WriteTo.Console();

            if (context.HostingEnvironment.IsDevelopment())
            {
                loggingConfiguration.MinimumLevel.Override("Catalog", Serilog.Events.LogEventLevel.Debug);
                loggingConfiguration.MinimumLevel.Override("Basket", Serilog.Events.LogEventLevel.Debug);
                loggingConfiguration.MinimumLevel.Override("Discount", Serilog.Events.LogEventLevel.Debug);
                loggingConfiguration.MinimumLevel.Override("Ordering", Serilog.Events.LogEventLevel.Debug);
            }
            var elasticUrl = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
            if (!string.IsNullOrEmpty(elasticUrl))
            {
                loggingConfiguration.WriteTo.Elasticsearch(
                    new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri(elasticUrl))
                    {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
                        IndexFormat = "ECommerce-Logs-{0:yyyy.MM.dd}",
                        MinimumLogEventLevel = Serilog.Events.LogEventLevel.Debug
                    });
            }
        };

    }
}
