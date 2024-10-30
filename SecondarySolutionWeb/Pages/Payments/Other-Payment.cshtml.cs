using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TheAgooProjectDataAccess;
using TheAgooProjectDataAccess.Data;
using TheAgooProjectModel;
using TheAgooProjectModel.ViewModels;

namespace SecondarySolutionWeb.Pages.Payments
{
    [Authorize(Roles = SD.IsStudent)]
    public class Other_PaymentModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public IList<AdditionalPayment> additionals { get; set; }
        public Other_PaymentModel(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void OnGet()
        {
            
        }
    }
}
