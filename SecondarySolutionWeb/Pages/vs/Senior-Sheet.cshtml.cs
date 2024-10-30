using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheAgooProjectDataAccess.Data;
using TheAgooProjectDataAccess;
using TheAgooProjectModel;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace SecondarySolutionWeb.Pages.vs
{
    [Authorize(Roles = SD.IsStudent)]
    public class Senior_SheetModel : PageModel
    {
		private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        public int NoInClass = 0, Position = 0;
		public double ToTalScore = 0;
		public StudentsData Studentsdata { get; set; }
		public TermRegistration termReg { get; set; }
		public GeneralClassTable _generalClass { get; set; }
		public RemarkPosition _remarkPosition { get; set; }
		public Senior_SheetModel(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
		{
			this.dbContext = dbContext;
            this.userManager = userManager;
        }
		public async Task<IActionResult> OnGet(int session, int classes, int subclass, string term)
		{
			if (ModelState.IsValid)
			{
				var ClaimsId = (ClaimsIdentity)User.Identity;
				var claim = ClaimsId.FindFirst(ClaimTypes.NameIdentifier);
				Studentsdata = dbContext.StudentTable.Include(k => k.ApplicationUser).FirstOrDefault(k => k.ApplicationUserId == claim.Value);
				if (Studentsdata == null)
				{
					TempData["error"] = "Unknown student";
					return RedirectToPage("/Index");
                }

                var admin = await userManager.GetUsersInRoleAsync(SD.Role_Admin);
                if (admin.Any())
                {
                    var newAdmin = admin.FirstOrDefault();
                    var settings = dbContext.Settings.FirstOrDefault(k => k.ApplicationUserId == newAdmin.Id);
                    if (!settings.CanPrintResult)
                    {
                        var fees = dbContext.PaymentReports.Include(l => l.Termregistration.StudentsData).FirstOrDefault(k => k.Termregistration.StudentsData.ApplicationUserId == claim.Value && k.Termregistration.Term == term && k.Termregistration.SessionYearId == session);
                        if (fees != null)
                        {
                            if (fees.Status != SD.Fees_Status_Completed)
                            {
                                TempData["error"] = "Our system has detected an incomplete fee, please visit your fees history for details";
                                return RedirectToPage("/Index");
                            }
                        }
                        else
                        {
                            TempData["error"] = "Our system has detected an incomplete fee, please visit your fees history for details";
                            return RedirectToPage("/Index");
                        }
                    }
                }

                var Results = dbContext.ResultTable.Include(k => k.Subjects).Include(k => k.Termregistration.StudentsData).Include(k => k.Termregistration.SessionYear).Include(k => k.Termregistration.SubClasses).Include(k => k.Termregistration.RemarkPositions).Include(k => k.Termregistration.StudentRatings).Include(k => k.Termregistration.Schoolclasses).Where(s => s.Termregistration.SessionYearId == session && s.Termregistration.ClassesInSchoolId == classes && s.Termregistration.SubClassId == subclass && s.Termregistration.Term == term).ToList();
				if (Results.Count() < 1)
				{
					TempData["error"] = "No result found for the selection, are you missing something?";
					return RedirectToPage("/Index");
				}

				NoInClass = Results.GroupBy(y => y.Termregistration.StudentId).Count();
				Studentsdata = dbContext.StudentTable.Find(Studentsdata.Id);
				termReg = Results.FirstOrDefault().Termregistration;
				_generalClass = dbContext.GeneralClassTables.Include(s => s.SessionYear).FirstOrDefault(k => k.SessionYearId == termReg.SessionYearId && k.Term == termReg.Term && k.ClassId == termReg.ClassesInSchoolId && k.SubClassId == termReg.SubClassId);
				if (_generalClass == null)
				{
					TempData["error"] = "Result still under computation, Check back later";
					return RedirectToPage("/Index");
				}
				_remarkPosition = Results.FirstOrDefault(k => k.Termregistration.StudentId == Studentsdata.Id).Termregistration.RemarkPositions;
				if (!_remarkPosition.R_Status || !_remarkPosition.P_Status)
				{
					TempData["error"] = "Result still under computation, Check back later";
					return RedirectToPage("/Index");
				}
				//WHAT IS THE CURRENT POSITION OF THIS STUDENT IN CLASS
				HighPosition = new List<HighLowScore>();
				foreach (var score in Results.GroupBy(y => y.Termregistration.StudentId))
				{
					var coll = new HighLowScore();
					coll.StudentId = score.FirstOrDefault().Termregistration.StudentId;
					foreach (var item in score)
					{
						coll.Score += (double)item.Total;
					}
					coll.Average = Math.Round(coll.Score / score.Count(), 2);
					HighPosition.Add(coll);
				}
				var getPosition = HighPosition.Rank(p => p.Average, (p, Rank) => new HighLowScore { Average = p.Average, Score = (double)p.Score, StudentId = p.StudentId, Rank = Rank }).ToList();
				Position = getPosition.FirstOrDefault(m => m.StudentId == Studentsdata.Id).Rank;
				ToTalScore = HighPosition.FirstOrDefault(k => k.StudentId == Studentsdata.Id).Score;
				//Narrow THE RESULT TO A PARTICULA STUDENT
				CollectDatas = new List<CollectData>();
				foreach (var item in Results.Where(k => k.Termregistration.StudentId == Studentsdata.Id))
				{
					var _highestLowest = new List<HighLowScore>();
					var collect = new CollectData()
					{
						SubjectId = item.SubjectId,
						SujectName = item.Subjects.Name,
						Assgmnt = item.Assignment,
						Test = item.Test,
						CWork = item.ClassWork,
						project = item.Project,
						Exams = item.Examination,
						TotalScores = item.Total
					};
					//GET SUBJECT AVERAGE
					var getsubject = Results.Where(j => j.SubjectId == item.SubjectId);
					collect.Highest = getsubject.Max(k => (double)k.Total);
					collect.Lowest = getsubject.Min(k => (double)k.Total);
					double gettotal = 0;
					foreach (var item1 in getsubject)
					{
						gettotal += (double)item1.Total;
					}
					collect.Average = Math.Round(gettotal / getsubject.Count(), 2);
					collect.Grade = SD.Grade((double)item.Total);
					collect.Remark = SD.Remark((double)item.Total);
					var getsubjec = getsubject.Rank(p => p.Total, (p, Rank) => new Averages { Score = (double)p.Total, StudentId = p.Termregistration.StudentId, Rank = Rank }).ToList();
					collect.Position = getsubjec.FirstOrDefault(k => k.StudentId == Studentsdata.Id).Rank;
					CollectDatas.Add(collect);
				}


				return Page();
			}
            TempData["error"] = "Incomplete information";
            return RedirectToPage("/Index");
		}

		public IList<CollectData> CollectDatas { get; set; }
		public IList<HighLowScore> HighLowPosition { get; set; }
		public IList<HighLowScore> HighPosition { get; set; }
		public class CollectData
		{
			public string SujectName { get; set; }
			public int SubjectId { get; set; }
			public double? Assgmnt { get; set; }
			public double? Test { get; set; }
			public double? CWork { get; set; }
			public double? project { get; set; }
			public double? Exams { get; set; }
			public double? TotalScores { get; set; }
			public double Average { get; set; }
			public double Highest { get; set; }
			public double Lowest { get; set; }
			public int Position { get; set; }
			public string Grade { get; set; }
			public string Remark { get; set; }
		}
		public class HighLowScore
		{
			public int StudentId { get; set; }
			public double Average { get; set; }
			public double Score { get; set; }
			public int Rank { get; set; }
		}
		public class Averages
		{
			public int StudentId { get; set; }
			public int SubjId { get; set; }
			public double Score { get; set; }
			public int Rank { get; set; }
		}
	}
}
