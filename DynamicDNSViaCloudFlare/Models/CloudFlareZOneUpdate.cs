namespace DynamicDNSViaCloudFlare.Models
{
    public class CloudFlareZOneUpdate
    {
        public string content { get; set; } = "";
        public string name { get; set; } = "";
        public bool proxied { get; set; } = false;
        public string type { get; set; } = "A";
        public string comment { get; set; } = "";
        public int ttl { get; set; } = 1;
    }
}
