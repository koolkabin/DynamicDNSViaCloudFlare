using DynamicDNSViaCloudFlare.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace DynamicDNSViaCloudFlare.Helpers
{
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
        public static PublicIPData GetPublicIp()
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
        public static string GetPublicIpString()
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
                            return ipAddress;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static async Task<List<CloudFlareZone>> ListCloudFlareZonesAsync()
        {
            if (cfSettings == null) { throw new Exception("CloudFlare Settings not initiated"); }

            string url = $"{cfSettings.BaseURL}/zones";
            HttpMethod hMethod = HttpMethod.Get;
            IDictionary<string, string> HeadersList = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {cfSettings.BearerToken}" },
                { "User-Agent", "PostmanRuntime/7.36.1" }
            };

            var client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var request = new HttpRequestMessage(hMethod, url);
            foreach (var item in HeadersList)
            {
                request.Headers.Add(item.Key, item.Value);
            }

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            var zones = JsonConvert.DeserializeObject<List<CloudFlareZone>>(responseContent);
            return zones;
        }
    }
}
