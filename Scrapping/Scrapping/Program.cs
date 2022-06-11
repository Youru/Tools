using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Scrapping.Domain.Interfaces;
using Scrapping.Interfaces;
using Scrapping.Services;
using Scrapping.Site;
using System;

namespace Scrapping
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {

                using IHost host = CreateHostBuilder(args).Build();

                var process = host.Services.GetRequiredService<Process>();
                process.Run(args).Wait();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddTransient<Process>();
            services.AddSingleton<IDocument, Document>();
            services.AddSingleton<IReplace, Replace>();
            services.AddSingleton<IScrappingService, AngleScrap>();
            services.AddSingleton<FactorySite>();
            FactorySite.Configures(ref services);
            services.AddLogging(loggingBuilder =>
            {
                // configure Logging with NLog
                loggingBuilder.ClearProviders();
                loggingBuilder.AddNLog();
            });

        });
    }
}