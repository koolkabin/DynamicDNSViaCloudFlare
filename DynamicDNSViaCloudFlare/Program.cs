using DynamicDNSViaCloudFlare.HangFireJobs;
using DynamicDNSViaCloudFlare.Models;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace DynamicDNSViaCloudFlare
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            ConfigureServices(builder, builder.Services);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // Configure Hangfire dashboard (optional)
            app.UseHangfireDashboard();

            // Configure Hangfire server
            app.UseHangfireServer();

            var myJob = new MyHangfireJob(builder.Configuration);
            RecurringJob.AddOrUpdate(() => myJob.Execute(), Cron.MinuteInterval(2));


            app.MapRazorPages();

            app.Run();
        }
        public static void ConfigureServices(WebApplicationBuilder builder, IServiceCollection services)
        {
            // Add Hangfire services
            services.AddHangfire(configuration => configuration
                .UseMemoryStorage()); // Use in-memory storage

            services.Configure<CloudFlareSettings>(builder.Configuration.GetSection("CloudFlareSettings"));
        }
    }
}