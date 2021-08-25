using System;
using System.Runtime.CompilerServices;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RulesEngineOnFunction.Repositories;
using RulesEngineOnFunction.Services;

[assembly: InternalsVisibleTo("RulesEngineOnFunction.Tests")]

namespace RulesEngineOnFunction
{
    /// <summary>
    /// Program.
    /// </summary>
    public class Program
    {
        private static readonly string CosmosDBConnectionString = Environment.GetEnvironmentVariable("CosmosDBConnectionString");
        private static readonly string DatabaseId = Environment.GetEnvironmentVariable("DatabaseId");
        private static readonly string ContainerId = Environment.GetEnvironmentVariable("ContainerId");

        /// <summary>
        /// Main.
        /// </summary>
        public static void Main()
        {
            CosmosClient client = new (CosmosDBConnectionString);
            CosmosRepository repository = new (client, DatabaseId, ContainerId);
            ErrorRuleEngine engine = new (repository);
            engine.Init().Wait();

            // DI: https://docs.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide#start-up-and-configuration
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(s =>
                {
                    s.AddSingleton<IErrorRuleEngine>(sp => engine);
                    s.AddSingleton<IRepository>(sp => repository);
                })
                .Build();

            host.Run();
        }
    }
}