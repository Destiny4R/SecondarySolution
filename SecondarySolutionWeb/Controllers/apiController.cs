using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TheAgooProjectDataAccess;
using TheAgooProjectDataAccess.Data;
using TheAgooProjectModel;

namespace SecondarySolutionWeb.Controllers
{
    [Authorize(Roles = SD.IsStudent)]
    public class apiController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ApplicationDbContext dbContext;

        public apiController(SignInManager<ApplicationUser> signInManager, ApplicationDbContext dbContext)
        {
            this.signInManager = signInManager;
            this.dbContext = dbContext;
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            if (signInManager.IsSignedIn(User))
            {
                await signInManager.SignOutAsync();
            }
            return RedirectToPage("/Account/Login");
        }
        [AcceptVerbs(["Post", "Get"])]
        public IActionResult FeesPayemtData()
        {
            try
            {
                var ClaimsId = (ClaimsIdentity)User.Identity;
                var claim = ClaimsId.FindFirst(ClaimTypes.NameIdentifier);
                var student = dbContext.StudentTable.FirstOrDefault(j => j.ApplicationUserId == claim.Value);
                IEnumerable<Payment> userData;
                userData = dbContext.Payments.Include(k => k.Termregistrations.SessionYear).Include(k => k.Termregistrations.Schoolclasses).Include(k => k.Termregistrations.SubClasses).Include(k => k.TermlyFees).Include(k => k.Termregistrations.StudentsData).Where(k=>k.Termregistrations.StudentId== student.Id).ToList();

                userData = userData.OrderByDescending(k => k.Id).ToList();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    userData = userData.OrderBy(s => sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    userData = dbContext.Payments.Include(k => k.Termregistrations.SessionYear).Include(k => k.Termregistrations.Schoolclasses).Include(k => k.Termregistrations.SubClasses).Include(k => k.TermlyFees).Include(k => k.Termregistrations.StudentsData).Where(m =>m.Termregistrations.StudentId == student.Id &&             (m.Termregistrations.Schoolclasses.Name.Contains(searchValue)
                                                || m.Termregistrations.SessionYear.Name.Contains(searchValue)
                                                || m.Termregistrations.StudentsData.SurName.Contains(searchValue)
                                                || m.Termregistrations.StudentsData.FirstName.Contains(searchValue)
                                                || m.Termregistrations.SessionYear.Name.Contains(searchValue)
                                                || m.Termregistrations.StudentsData.StudentRegNo.Contains(searchValue)
                                                || m.ReceiptNo.Contains(searchValue)
                                                || m.Termregistrations.Term.Contains(searchValue)));
                }
                recordsTotal = userData.Count();
                var data = userData.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [AcceptVerbs(["Post", "Get"])]
        public IActionResult PTAFeesPayemtData()
        {
            try
            {
                var ClaimsId = (ClaimsIdentity)User.Identity;
                var claim = ClaimsId.FindFirst(ClaimTypes.NameIdentifier);
                var student = dbContext.StudentTable.FirstOrDefault(j => j.ApplicationUserId == claim.Value);

                IEnumerable<PTATable> userData;
                userData = dbContext.PTATables.Include(k => k.Termregistration.SessionYear).Include(k => k.Termregistration.Schoolclasses).Include(k => k.Termregistration.SubClasses).Include(k => k.PTAFee).Include(k => k.Termregistration.StudentsData).Where(k => k.Termregistration.StudentId == student.Id).ToList();

                userData = userData.OrderByDescending(k => k.Id).ToList();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    userData = userData.OrderBy(s => sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    userData = dbContext.PTATables.Include(k => k.Termregistration.SessionYear).Include(k => k.Termregistration.Schoolclasses).Include(k => k.Termregistration.SubClasses).Include(k => k.PTAFee).Include(k => k.Termregistration.StudentsData).Where(m =>m.Termregistration.StudentId == student.Id &&(m.Termregistration.Schoolclasses.Name.Contains(searchValue)
                                                || m.Termregistration.SessionYear.Name.Contains(searchValue)
                                                || m.Termregistration.StudentsData.SurName.Contains(searchValue)
                                                || m.Termregistration.StudentsData.FirstName.Contains(searchValue)
                                                || m.Termregistration.SessionYear.Name.Contains(searchValue)
                                                || m.Termregistration.StudentsData.StudentRegNo.Contains(searchValue)
                                                || m.ReceiptNo.Contains(searchValue)
                                                || m.Termregistration.Term.Contains(searchValue)));
                }
                recordsTotal = userData.Count();
                var data = userData.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [AcceptVerbs(["Post", "Get"])]
        public IActionResult otherPayemrnt()
        {
            try
            {
                var ClaimsId = (ClaimsIdentity)User.Identity;
                var claim = ClaimsId.FindFirst(ClaimTypes.NameIdentifier);
                var userData = dbContext.AdditionalPayments.Include(k => k.SpecialPay).Include(i => i.Termregistration.SessionYear).Include(i => i.Termregistration.Schoolclasses).Include(i => i.Termregistration.SubClasses).Include(i => i.Termregistration.StudentsData).Include(m=>m.SpecialPay.OtherPayTables).Where(k => k.Termregistration.StudentsData.ApplicationUserId == claim.Value).ToList();

                userData = userData.OrderByDescending(k => k.Id).ToList();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    userData = userData.OrderBy(s => sortColumn + " " + sortColumnDirection).ToList();
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    userData = dbContext.AdditionalPayments.Include(k => k.SpecialPay).Include(i => i.Termregistration.SessionYear).Include(i => i.Termregistration.Schoolclasses).Include(i => i.Termregistration.SubClasses).Include(i => i.Termregistration.StudentsData).Where(m => m.SpecialPay.OtherPayTables.Name.Contains(searchValue)||(m.Termregistration.Schoolclasses.Name.Contains(searchValue)
                                                || m.Termregistration.SessionYear.Name.Contains(searchValue)
                                                || m.Termregistration.StudentsData.SurName.Contains(searchValue)
                                                || m.Termregistration.StudentsData.FirstName.Contains(searchValue)
                                                || m.Termregistration.SessionYear.Name.Contains(searchValue)
                                                || m.Termregistration.StudentsData.StudentRegNo.Contains(searchValue)
                                                || m.Termregistration.Term.Contains(searchValue))).ToList();
                }
                recordsTotal = userData.Count();
                var data = userData.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
    }
}
