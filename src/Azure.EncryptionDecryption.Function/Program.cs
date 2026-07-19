using Azure.EncryptionDecryption.Utilities;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = FunctionsApplication.CreateBuilder(args);

IConfiguration config = BuildConfiguration(builder.Environment.ContentRootPath);

static IConfiguration BuildConfiguration(string contentRootPath)
{
    var config = new ConfigurationBuilder()
        .SetBasePath(contentRootPath)
        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

    return config;
}

var functionAppName = Environment.GetEnvironmentVariable("FUNCTION_APP_NAME");

builder.ConfigureFunctionsWebApplication();

builder.Services.AddUtilities();

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

builder.Build().Run();
