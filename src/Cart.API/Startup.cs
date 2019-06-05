using System;
using System.Threading.Tasks;
using GrainInterfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Cart.API
{
    public class Startup
    {
        private static readonly string AzureConnectionString = "DefaultEndpointsProtocol=https;AccountName=orleansstorage;AccountKey=+NuxKTXei7RwvIbwDQSba2MJYMUM2nXmEVpT6SoGuZuW1rqXhocnqKJEhQG2OmuPVaX6JaQsndEcC4vOBD7dXg==;EndpointSuffix=core.windows.net";
        private static readonly string DebugConnectionString = "UseDevelopmentStorage=true";

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton(CreateClusterClient);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }

        private IClusterClient CreateClusterClient(IServiceProvider serviceProvider)
        {
            //TODO: move magic strings?
            var client = new ClientBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "orleans-wdm4-cluster";
                    options.ServiceId = "orleans-wdm4-service";
                })
                .UseAzureStorageClustering(opt => opt.ConnectionString = AzureConnectionString)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IOrderGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();
            
            Console.WriteLine("API CLUSTERCLIENT CONNECTION ATTEMPT STARTED");
            client.Connect(RetryFilter).GetAwaiter().GetResult();
            return client;

            //https://github.com/dotnet/orleans/issues/5158
            async Task<bool> RetryFilter(Exception exception)
            {
                Console.WriteLine("Exception while attempting to connect to Orleans cluster: {Exception}", exception);
                await Task.Delay(TimeSpan.FromSeconds(2));
                return true;
            }
        }
    }
}
