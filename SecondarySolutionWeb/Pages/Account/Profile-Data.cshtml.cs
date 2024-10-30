using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TheAgooProjectDataAccess;
using TheAgooProjectDataAccess.Data;
using TheAgooProjectModel;

namespace SecondarySolutionWeb.Pages.Account
{
    [Authorize(Roles = SD.IsStudent)]
    public class Profile_DataModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;

        public StudentsData student { get; set; }
        public ParentStudent parent { get; set; }
        public Profile_DataModel(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void OnGet()
        {
            var ClaimsId = (ClaimsIdentity)User.Identity;
            var claim = ClaimsId.FindFirst(ClaimTypes.NameIdentifier);
            student = dbContext.StudentTable.FirstOrDefault(j => j.ApplicationUserId == claim.Value);
            if (student != null)
            {
                parent = dbContext.ParentStudents.Include(k=>k.ParentTable).FirstOrDefault(k => k.StudentsdataId == student.Id);
            }
        }
    }
}
