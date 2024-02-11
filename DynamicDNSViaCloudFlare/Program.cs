using DynamicDNSViaCloudFlare.Helpers;
using DynamicDNSViaCloudFlare.Models;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
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

            builder.ConfigureServices(builder.Services);

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

            app.ConfigureHangFireJobs(builder);

            app.MapRazorPages();

            app.Run();
        }




    }
}