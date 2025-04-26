namespace DynamicDNSViaCloudFlare.Models
{
    public class CloudFlareZone
    {
        public string ZoneID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int PlanLevel { get; set; }
        public bool Paused { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
