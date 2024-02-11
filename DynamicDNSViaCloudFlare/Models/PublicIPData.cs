namespace DynamicDNSViaCloudFlare.Models
{
    public class PublicIPData
    {
        public bool status { get; set; }
        public string IP { get; set; }
        public string ErrorMsg { get; set; }
        public static string LastIP { get; set; } = "";
    }

    public class EmailSettings
    {
        public string ReportTo { get; set; }
        public Outgoingserver OutgoingServer { get; set; }
    }

    public class Outgoingserver
    {
        public string Server { get; set; }
        public string UserName { get; set; }
        public string FromAddress { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public bool HasSSL { get; set; }
        public string SSLVersion { get; set; }
    }
}
