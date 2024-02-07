namespace DynamicDNSViaCloudFlare.Models
{
    public class CloudFlareSettings
    {
        public string BaseURL { get; set; }
        public string BearerToken { get; set; }
        public string DNS_Record_ID { get; set; }
        public string DNS_Record_Name { get; set; }
        public string ZoneID { get; set; }
    }
}
