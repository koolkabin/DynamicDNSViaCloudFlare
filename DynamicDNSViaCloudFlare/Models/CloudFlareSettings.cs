﻿namespace DynamicDNSViaCloudFlare.Models
{
    public class CloudFlareSettings
    {
        public string BaseURL { get; set; }
        public string BearerToken { get; set; }
        public bool SyncDNSRecord { get; set; }
        public string DNS_Record_ID { get; set; }
        public string DNS_Record_Name { get; set; }
        public string ZoneID { get; set; }
        public string ApiKey { get; set; }
        public string Email { get; set; }
    }

}
