using DynamicDNSViaCloudFlare.Helpers;
using DynamicDNSViaCloudFlare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DynamicDNSViaCloudFlare.Pages
{
    public class ZonesModel : PageModel
    {
        private readonly ILogger<ZonesModel> _logger;
        private IConfiguration _config;
        public List<CloudFlareZone> Zones { get; set; } = new List<CloudFlareZone>();

        public ZonesModel(ILogger<ZonesModel> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task OnGet()
        {
            try
            {
                HTTPCloudFlareClientHelper.InitConfig(_config);
                Zones = await HTTPCloudFlareClientHelper.ListCloudFlareZonesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching zones from CloudFlare.");
            }
        }
    }
}