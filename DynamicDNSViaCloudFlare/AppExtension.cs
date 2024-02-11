using DynamicDNSViaCloudFlare.HangFireJobs;
using DynamicDNSViaCloudFlare.Helpers;
using DynamicDNSViaCloudFlare.Models;
using Hangfire;
using Hangfire.MemoryStorage;
using System.Configuration;

namespace DynamicDNSViaCloudFlare
{
    public static class AppExtension
    {
        public static void ConfigureHangFireJobs(this WebApplication app, WebApplicationBuilder builder)
        {
            using (var serviceScope = app.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var emailHelper = services.GetRequiredService<EmailHelper>();
                var myJob = new MyHangfireJob(builder.Configuration, emailHelper);
                RecurringJob.AddOrUpdate(() => myJob.Execute(), Cron.MinuteInterval(2));
            }

        }
        public static void ConfigureServices(this WebApplicationBuilder builder, IServiceCollection services)
        {
            // Add Hangfire services
            services.AddHangfire(configuration => configuration
                .UseMemoryStorage()); // Use in-memory storage
                                      // Configure AppSettings
            CloudFlareSettings cfAppSettings = new CloudFlareSettings();
            builder.Configuration.GetSection("CloudFlareSettings").Bind(cfAppSettings);
            services.AddSingleton(cfAppSettings);

            EmailSettings emailSettings = new EmailSettings();
            builder.Configuration.GetSection("EmailSettings").Bind(emailSettings);
            services.AddSingleton(emailSettings);

            services.AddSingleton<EmailHelper>();
        }
    }
}