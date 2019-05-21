using Grains;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace Silo
{
    public class Program
    {
        private static ISiloHost silo;
        private static readonly ManualResetEvent siloStopped = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            silo = new SiloHostBuilder()
                // .UseLocalhostClustering(serviceId:"blog-orleans-deepdive")
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "orleans-docker";
                    options.ServiceId = "sample-app";
                })
                .AddAdoNetGrainStorage("CartStorage", options =>
                    {
                        options.Invariant = "MySql.Data.MySqlClient";
                        options.ConnectionString = "Server=mariadb;Database=cart;Uid=root;Pwd=P@55w0rd";
                        options.UseJsonFormat = true;
                    })
                //.AddMemoryGrainStorage("CartStorage")
                .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(CartGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole())
                //.UseDashboard()
                .Build();

            Task.Run(StartSilo);

            AssemblyLoadContext.Default.Unloading += context =>
            {
                Task.Run(StopSilo);
                siloStopped.WaitOne();
            };

            siloStopped.WaitOne();
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