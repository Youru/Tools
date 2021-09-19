using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using ScrappingNewTest.Interfaces;
using ScrappingNewTest.Services;
using ScrappingNewTest.Services.Site.Novel;
using ScrappingNewTest.Services.Site.Scan;
using System;

namespace ScrappingNewTest
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
            services.AddSingleton<IAngleScrap, AngleScrap>();
            services.AddSites();
            services.AddLogging(loggingBuilder =>
            {
                // configure Logging with NLog
                loggingBuilder.ClearProviders();
                loggingBuilder.AddNLog();
            });

        });
    }

    public static class ServiceExtension
    {
        public static IServiceCollection AddSites(this IServiceCollection services)
        {

            services.AddScoped<BaseNovel>();
            services.AddScoped<Gravitytales>();
            services.AddScoped<NovelFull>();
            services.AddScoped<WebNovel>();
            services.AddScoped<WuxiaWorld>();

            services.AddScoped<BaseScan>();
            services.AddScoped<MangaLel>();

            services.AddTransient<Func<string, ISite>>(serviceprovider => (key) =>
                 key switch
                 {
                     "scan" => serviceprovider.GetService<BaseScan>(),
                     "novel" => serviceprovider.GetService<BaseNovel>(),
                     "gravitytales" => serviceprovider.GetService<Gravitytales>(),
                     "wuxiaworld" => serviceprovider.GetService<WuxiaWorld>(),
                     "mangalel" => serviceprovider.GetService<MangaLel>(),
                     "webnovel" => serviceprovider.GetService<WebNovel>(),
                     "novelfull" => serviceprovider.GetService<NovelFull>(),
                     _ => throw new ArgumentException("Invalid type value ", paramName: key)
                 }
            );

            return services;
        }
    }
}