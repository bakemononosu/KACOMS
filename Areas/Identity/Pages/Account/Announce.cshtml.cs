using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ElsWebApp.Areas.Identity.Pages.Account
{
    public class AnnounceModel : PageModel
    {
        public string MessageType { get; set; } = string.Empty;
        public string ProcMessage { get; set; } = string.Empty;
        public string UrlLink { get; set; } = string.Empty;

        public void OnGet(string type, string message, string link = "")
        {
            MessageType = type;
            ProcMessage = message;
            UrlLink = link;
        }
    }
}
