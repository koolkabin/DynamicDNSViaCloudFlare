using DynamicDNSViaCloudFlare.Helpers;
using DynamicDNSViaCloudFlare.Models;
using Hangfire.MemoryStorage.Database;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace DynamicDNSViaCloudFlare.HangFireJobs
{
    public class MyHangfireJob
    {
        private readonly IConfiguration _configuration;
        private readonly EmailHelper _emailHelper;
        public MyHangfireJob(IConfiguration configuration, EmailHelper emailHelper)
        {
            _configuration = configuration;
            _emailHelper = emailHelper;
        }

        public async Task Execute()
        {
            HTTPCloudFlareClientHelper.InitConfig(_configuration);

            PublicIPData d1 = HTTPCloudFlareClientHelper.GetPublicIp();
            if (!d1.status) { throw new Exception(d1.ErrorMsg); }
            Console.WriteLine("Public IP: " + d1.IP);
            string oldIP = PublicIPData.LastIP;
            if (d1.IP == oldIP)
            {
                Console.WriteLine($"Matched Previous IP {oldIP}. So no need to update.");
                return;
            }
            PublicIPData.LastIP = d1.IP;

            CloudFlareZOneUpdate r2 = new CloudFlareZOneUpdate()
            {
                content = d1.IP,
                name = HTTPCloudFlareClientHelper.cfSettings.DNS_Record_Name
            };

            await HTTPCloudFlareClientHelper.MakeHTTPCall(r2);
            _emailHelper.SendEmail($"New IP {d1.IP} has been updated.", $"DNS IP Change from {oldIP} to {d1.IP}");
        }

    }
}
