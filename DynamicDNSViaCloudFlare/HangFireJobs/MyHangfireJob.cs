using DynamicDNSViaCloudFlare.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace DynamicDNSViaCloudFlare.HangFireJobs
{
    public class PublicIPData
    {
        public bool status { get; set; }
        public string IP { get; set; }
        public string ErrorMsg { get; set; }
    }
    public class CloudFlareZOneUpdate
    {
        public string content { get; set; } = "";
        public string name { get; set; } = "";
        public bool proxied { get; set; } = false;
        public string type { get; set; } = "A";
        public string comment { get; set; } = "";
        public int ttl { get; set; } = 3600;
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
            // Read cloudflareSettings from appsettings.json
            CloudFlareSettings cloudflareSettings = _configuration
                .GetSection("CloudFlareSettings")
                .Get<CloudFlareSettings>();

            // Use settings in your Hangfire job
            await MakeHTTPCall(cloudflareSettings);
            // Example usage
            // CloudflareApiClient client = new CloudflareApiClient(apiKey, email);
            // client.DoSomething();
        }
        private async Task MakeHTTPCall(CloudFlareSettings data)
        {
            PublicIPData d1 = GetPublicIp();
            if (!d1.status) { throw new Exception(d1.ErrorMsg); }
            Console.WriteLine("Public IP: " + d1.IP);

            CloudFlareZOneUpdate r2 = new CloudFlareZOneUpdate() { content = d1.IP, name = data.DNS_Record_Name };

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Put, $"{data.BaseURL}/zones/{data.ZoneID}/dns_records/{data.DNS_Record_ID}");
            request.Headers.Add("Authorization", $"Bearer {data.BearerToken}");
            request.Headers.Add("User-Agent", "PostmanRuntime/7.36.1");
            string json = JsonConvert.SerializeObject(r2);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            //response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();

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
}
