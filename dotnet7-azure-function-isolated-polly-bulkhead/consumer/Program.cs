using consumer.TypedHttpClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        var currentDirectory = hostingContext.HostingEnvironment.ContentRootPath;

        config.SetBasePath(currentDirectory)
            .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
        config.Build();
    })
    .ConfigureServices((services) =>
    {
        var bulkheadPolicy = Policy
            .BulkheadAsync<HttpResponseMessage>(maxParallelization: 2, maxQueuingActions: 2, _ =>
            {
                Console.Out.WriteLine("Bulkhead policy rejected the request");
                return Task.CompletedTask;
            });

        services.AddHttpClient<DelayHttpClient>()
            .AddPolicyHandler(bulkheadPolicy);
    })
    .Build();

host.Run();
