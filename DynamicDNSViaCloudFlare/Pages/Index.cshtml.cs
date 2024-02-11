using DynamicDNSViaCloudFlare.Helpers;
using DynamicDNSViaCloudFlare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DynamicDNSViaCloudFlare.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public string LastIP { get; set; }
        public string CurrentIP { get; set; }
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;

        }

        public void OnGet()
        {
            LastIP = PublicIPData.LastIP;
            CurrentIP = HTTPCloudFlareClientHelper.GetPublicIp().IP;
        }
    }
}