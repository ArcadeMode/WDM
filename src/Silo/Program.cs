using Grains;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net.Http;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;


namespace Silo
{
    public class Program
    {
        private static ISiloHost silo;
        private static readonly ManualResetEvent siloStopped = new ManualResetEvent(false);
        private static readonly string AzureConnectionString = "DefaultEndpointsProtocol=https;AccountName=orleansstorage;AccountKey=+NuxKTXei7RwvIbwDQSba2MJYMUM2nXmEVpT6SoGuZuW1rqXhocnqKJEhQG2OmuPVaX6JaQsndEcC4vOBD7dXg==;EndpointSuffix=core.windows.net";
        private static readonly string DebugConnectionString = "UseDevelopmentStorage=true";

        static async Task Main(string[] args)
        {
            silo = new SiloHostBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "orleans-wdm4-cluster-marc3";
                    options.ServiceId = "orleans-wdm4-service-marc3";
                })
                .UseAzureStorageClustering(opt => opt.ConnectionString = AzureConnectionString)
                .AddAzureTableGrainStorage("ItemStorage", ConfigureAzureTableStorage)
                .AddAzureTableGrainStorage("OrderStorage", ConfigureAzureTableStorage)
                .AddAzureTableGrainStorage("UserStorage", ConfigureAzureTableStorage)
                .AddAzureTableGrainStorage("ItemStorage", ConfigureAzureTableStorage)
                .AddAzureTableGrainStorage("UserStorage", ConfigureAzureTableStorage)
                .AddAzureTableGrainStorage("OrderStorage", ConfigureAzureTableStorage)
                .AddAzureTableGrainStorage("PaymentStorage", ConfigureAzureTableStorage)
                .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(OrderGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole())
                .UseDashboard()
                .UseTransactionalState()
                .AddAzureTableGrainStorageAsDefault(options => options.ConnectionString = AzureConnectionString)
                //.UseInClusterTransactionManager()
                //.UseAzureTransactionLog(options => options.ConnectionString = AzureConnectionString) 
                .Build();

            Task.Run(StartSilo);

            AssemblyLoadContext.Default.Unloading += context =>
            {
                Console.WriteLine("Stopping silo");
                Task.Run(StopSilo);
                siloStopped.WaitOne();
            };

            siloStopped.WaitOne();
        }

        private static void ConfigureAzureTableStorage(AzureTableStorageOptions ob)
        {
            ob.ConnectionString = AzureConnectionString;
            ob.DeleteStateOnClear = true;
        }

        private static async Task StartSilo()
        {
            await silo.StartAsync();
            Console.WriteLine("Silo started");
        }

        private static async Task StopSilo()
        {
            await silo.StopAsync();
            Console.WriteLine("Silo stopped");
            siloStopped.Set();
        }
    }
}
