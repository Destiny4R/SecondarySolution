using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.ObjectModel;
using TheAgooProjectDataAccess.Data;
using TheAgooProjectDataAccess;
using TheAgooProjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.AspNetCore.Identity;

namespace SecondarySolutionWeb.Pages.vs
{
    [Authorize(Roles = SD.IsStudent)]
    public class Annual_SheetModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public StudentsData Studentsdata { get; set; }
        public GeneralClassTable _generalClass { get; set; }
        public RemarkPosition _remarkPosition { get; set; }
        public Annual_SheetModel(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }
        public int termly = 0, NoinClass = 0, PositionInClass = 0, activeTerms = 0;
        public async Task<IActionResult> OnGet(int session)
        {
            if(session < 0)
            {
                TempData["error"] = "Please select session before proceeding";
                return RedirectToPage("/Index");
            }
            var ClaimsId = (ClaimsIdentity)User.Identity;
            var claim = ClaimsId.FindFirst(ClaimTypes.NameIdentifier);

            var admin = await userManager.GetUsersInRoleAsync(SD.Role_Admin);
            if (admin.Any())
            {
                var newAdmin = admin.FirstOrDefault();
                var settings = dbContext.Settings.FirstOrDefault(k => k.ApplicationUserId == newAdmin.Id);
                if (!settings.CanPrintResult)
                {
                    var fees = dbContext.PaymentReports.Include(l => l.Termregistration.StudentsData).Where(k => k.Termregistration.StudentsData.ApplicationUserId == claim.Value && k.Termregistration.SessionYearId == session).ToList();
                    if (fees.Any())
                    {
                        int go = 0;
                        foreach (var item in fees)
                        {
                            if (item?.Status == SD.Fees_Status_Completed)
                            {
                                go++;
                            }
                        }
                        if (go != 3)
                        {
                            TempData["error"] = "Our system has detected an incomplete fees in one or more term, please visit your fees history for details";
                            return RedirectToPage("/Index");
                        }
                    }
                    else
                    {
                        TempData["error"] = "Our system has detected an incomplete fees in one or more term, please visit your fees history for details";
                        return RedirectToPage("/Index");
                    }
                }
            }

            
            var user = dbContext.StudentTable.Include(k=>k.ApplicationUser).FirstOrDefault(k => k.ApplicationUserId == claim.Value);
            if (user==null)
            {
                TempData["error"] = "Unknown student";
                return RedirectToPage("/Index");
            }else
            {
                var termReg = dbContext.TermRegistrations.FirstOrDefault(h => h.SessionYearId == session && h.StudentId == user.Id);
                if (termReg == null)
                {
                    TempData["error"] = "Sorry! this request cannot be process at this time, some information are missing";
                    return RedirectToPage("Annual-Master");
                }

                var Results = dbContext.ResultTable.Include(k => k.Subjects).Include(k => k.Termregistration.StudentsData).Include(k => k.Termregistration.SessionYear).Include(k => k.Termregistration.SubClasses).Include(k => k.Termregistration.Schoolclasses).Where(s => s.Termregistration.SessionYearId == session && s.Termregistration.ClassesInSchoolId == termReg.ClassesInSchoolId && s.Termregistration.SubClassId == termReg.SubClassId).ToList();
                if (Results.Count() < 1)
                {
                    TempData["error"] = "Sorry! this request cannot be process at this time, some information are missing";
                    return RedirectToPage("/");
                }
                NoinClass = Results.GroupBy(k => k.Termregistration.StudentId).Count();


                Studentsdata = termReg.StudentsData;
                activeTerms = Results.GroupBy(j => j.Termregistration.Term).Count();
                _generalClass = dbContext.GeneralClassTables.Include(s => s.SessionYear).FirstOrDefault(k => k.SessionYearId == termReg.SessionYearId && k.Term == "Third" && k.ClassId == termReg.ClassesInSchoolId && k.SubClassId == termReg.SubClassId);
                if (_generalClass == null)
                {
                    _generalClass = dbContext.GeneralClassTables.Include(s => s.SessionYear).FirstOrDefault(k => k.SessionYearId == termReg.SessionYearId && k.Term == "Second" && k.ClassId == termReg.ClassesInSchoolId && k.SubClassId == termReg.SubClassId);
                }
                if (_generalClass == null)
                {
                    _generalClass = dbContext.GeneralClassTables.Include(s => s.SessionYear).FirstOrDefault(k => k.SessionYearId == termReg.SessionYearId && k.Term == "First" && k.ClassId == termReg.ClassesInSchoolId && k.SubClassId == termReg.SubClassId);
                }
                if (_generalClass == null)
                {
                    TempData["error"] = "Result not available for the selected information, as general class information for either term has not been added";
                    return RedirectToPage("/");
                }
                int TReg = Results.FirstOrDefault(k => k.Termregistration.StudentId == user.Id).TermRegId;
                _remarkPosition = await dbContext.RemarkPositions.FirstOrDefaultAsync(k => k.TermRegId == TReg);
                int termz = Results.GroupBy(h => h.Termregistration.Term).Count();

                CollectDatas = new List<CollectData>();
                foreach (var item in Results.Where(m => m.Termregistration.StudentId == user.Id).GroupBy(n => n.SubjectId))
                {
                    CollectData CollectData = new CollectData();
                    CollectData.SujectName = item.FirstOrDefault().Subjects.Name;
                    CollectData.SubjectId = item.FirstOrDefault().Subjects.Id;
                    if (item.FirstOrDefault(k => k.Termregistration.Term == SD.First) != null)
                    {
                        CollectData.First = item.FirstOrDefault(k => k.Termregistration.Term == SD.First)?.Total ?? 0;
                        //termly++;
                    }
                    else { CollectData.First = 0; }
                    if (item.FirstOrDefault(k => k.Termregistration.Term == SD.Second) != null)
                    {
                        CollectData.Second = item.FirstOrDefault(k => k.Termregistration.Term == SD.Second)?.Total ?? 0;
                        //termly++;
                    }
                    else { CollectData.Second = 0; }
                    if (item.FirstOrDefault(k => k.Termregistration.Term == SD.Third) != null)
                    {
                        CollectData.Third = item.FirstOrDefault(k => k.Termregistration.Term == SD.Third)?.Total ?? 0;
                        //termly++;
                    }
                    else { CollectData.Third = 0; }
                    CollectData.TotalScores = CollectData.First + CollectData.Second + CollectData.Third;
                    double scores = CollectData.First + CollectData.Second + CollectData.Third;
                    termly = item.GroupBy(k => k.Termregistration.Term).Count();
                    CollectData.YearlyAv = Math.Round((scores / termly), 2);

                    var NewResult = Results.Where(k => k.SubjectId == item.FirstOrDefault().SubjectId);
                    var highestList = new Collection<HighLowScore>();
                    foreach (var resultx in NewResult.GroupBy(M => M.Termregistration.StudentId))
                    {
                        var highest = new HighLowScore();
                        highest.StudentId = resultx.FirstOrDefault().Termregistration.StudentId;
                        foreach (var scorez in resultx)
                        {
                            highest.Score += scorez?.Total ?? 0.0;
                        }
                        highestList.Add(highest);
                    }

                    CollectData.Highest = highestList.Max(k => k.Score);
                    CollectData.Lowest = highestList.Min(k => k.Score);

                    //USE THE highestlist COLLECTION TO DETERMINE THIS VERY STUDENT POSITION IN THIS SUBJECT

                    var rankingsubject = highestList.Rank(p => p.Score, (p, Rank) => new HighLowScore { Score = p.Score, StudentId = p.StudentId, Rank = Rank }).ToList(); ;
                    var postionx = rankingsubject.FirstOrDefault(k => k.StudentId == item.FirstOrDefault().Termregistration.StudentId);
                    if (postionx != null)
                    {
                        CollectData.Position = postionx.Rank;
                    }
                    CollectData.Grade = SD.Annual_Calculate_Grade2(CollectData.First + CollectData.Second + CollectData.Third, termz);
                    CollectData.Remark = SD.Annual_Calculate_Remark2(CollectData.First + CollectData.Second + CollectData.Third, termz);

                    CollectDatas.Add(CollectData);
                }
                HighPosition = new List<HighLowScore>();
                foreach (var item in Results.GroupBy(m => m.Termregistration.StudentId))
                {
                    var post = new HighLowScore();
                    post.StudentId = item.FirstOrDefault().Termregistration.StudentId;
                    foreach (var item2 in item)
                    {
                        post.Score += item2?.Total ?? 0.0;
                    }
                    post.Average = Math.Round(post.Score / item.Count(), 2);
                    HighPosition.Add(post);
                }
                HighPosition = HighPosition.Rank(p => p.Average, (p, Rank) => new HighLowScore { Average = p.Average, Score = p.Score, StudentId = p.StudentId, Rank = Rank }).ToList();
                PositionInClass = HighPosition.FirstOrDefault(k => k.StudentId == user.Id).Rank;
                return Page();
            }
            TempData["error"] = "Sorry! this request cannot be process at this time, some information are missing";
            return RedirectToPage("Annual-Master");
        }
        public IList<CollectData> CollectDatas { get; set; }
        public IList<HighLowScore> HighLowScores { get; set; }
        public IList<HighLowScore> HighPosition { get; set; }
        public class CollectData
        {
            public string SujectName { get; set; }
            public int SubjectId { get; set; }
            public double First { get; set; }
            public double Second { get; set; }
            public double Third { get; set; }
            public double TotalScores { get; set; }
            public double YearlyAv { get; set; }
            public double Highest { get; set; }
            public double Lowest { get; set; }
            public int Position { get; set; }
            public string Grade { get; set; }
            public string Remark { get; set; }
        }
        public class HighLowScore
        {
            public int StudentId { get; set; }
            public double Score { get; set; }
            public double Average { get; set; }
            public int Rank { get; set; }
        }
    }
}
