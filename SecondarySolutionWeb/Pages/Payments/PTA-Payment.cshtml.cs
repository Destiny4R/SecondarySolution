using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheAgooProjectDataAccess;

namespace SecondarySolutionWeb.Pages.Payments
{
    [Authorize(Roles = SD.IsStudent)]
    public class PTA_PaymentModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
