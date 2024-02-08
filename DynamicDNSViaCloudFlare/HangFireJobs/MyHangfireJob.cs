using DynamicDNSViaCloudFlare.Models;
using Hangfire.MemoryStorage.Database;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Configuration;
using System.Net;
using System.Text;

namespace DynamicDNSViaCloudFlare.HangFireJobs
{
    public class PublicIPData
    {
        public bool status { get; set; }
        public string IP { get; set; }
        public string ErrorMsg { get; set; }
        public static string LastIP { get; set; } = "";
    }
    public class CloudFlareZOneUpdate
    {
        public string content { get; set; } = "";
        public string name { get; set; } = "";
        public bool proxied { get; set; } = false;
        public string type { get; set; } = "A";
        public string comment { get; set; } = "";
        public int ttl { get; set; } = 1;
    }
    public class MyHangfireJob
    {
        private readonly IConfiguration _configuration;

        public MyHangfireJob(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Execute()
        {
            HTTPCloudFlareClientHelper.InitConfig(_configuration);

            PublicIPData d1 = GetPublicIp();
            if (!d1.status) { throw new Exception(d1.ErrorMsg); }
            Console.WriteLine("Public IP: " + d1.IP);
            if (d1.IP == PublicIPData.LastIP)
            {
                Console.WriteLine($"Matched Previous IP {PublicIPData.LastIP}. So no need to update.");
                return;
            }
            PublicIPData.LastIP = d1.IP;

            CloudFlareZOneUpdate r2 = new CloudFlareZOneUpdate()
            {
                content = d1.IP,
                name = HTTPCloudFlareClientHelper.cfSettings.DNS_Record_Name
            };

            await HTTPCloudFlareClientHelper.MakeHTTPCall(r2);

        }
        public PublicIPData GetPublicIp()
        {
            try
            {
                // Query an external service to get the public IP address
                string url = "https://api.ipify.org";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(stream);
                            string ipAddress = reader.ReadToEnd();
                            return new PublicIPData() { status = true, IP = ipAddress };
                        }
                    }
                    else
                    {
                        return new PublicIPData() { status = false, ErrorMsg = response.ToString() };
                    }
                }
            }
            catch (Exception ex)
            {
                return new PublicIPData() { status = false, ErrorMsg = ex.Message };
            }
        }
    }
    public static class HTTPCloudFlareClientHelper
    {
        public static CloudFlareSettings cfSettings = null;
        public static void InitConfig(IConfiguration configuration, bool reInit = false)
        {
            if (cfSettings != null && reInit == false) { return; }
            // Read cloudflareSettings from appsettings.json
            cfSettings = configuration
                .GetSection("CloudFlareSettings")
                .Get<CloudFlareSettings>();
        }
        public static async Task MakeHTTPCall<Req>(Req r2)
        {
            if (cfSettings == null) { throw new Exception("CloudFlare Settings not initated"); }

            string url = $"{cfSettings.BaseURL}/zones/{cfSettings.ZoneID}/dns_records/{cfSettings.DNS_Record_ID}";
            HttpMethod hMethod = HttpMethod.Put;
            IDictionary<string, string> HeadersList = new Dictionary<string, string>();
            HeadersList.Add("Authorization", $"Bearer {cfSettings.BearerToken}");
            HeadersList.Add("User-Agent", $"PostmanRuntime/7.36.1");

            var client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var request = new HttpRequestMessage(hMethod, url);
            foreach (var item in HeadersList)
            {
                request.Headers.Add(item.Key, item.Value);
            }
            //request.Headers.Add("Authorization", $"Bearer {cfSettings.BearerToken}");
            //request.Headers.Add("User-Agent", "PostmanRuntime/7.36.1");
            string json = JsonConvert.SerializeObject(r2);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            //response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
        }

    }
}
