using FeeManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.WebPages;

namespace FeeManagement.Controllers
{
    public class HomeController : Controller
    {
        OurDbContext db = new OurDbContext();
        
        // Fees List //

        public ActionResult FeeList()
        {
            var Feel = db.AddFees.ToList().OrderByDescending(x => x.Add_Fee_Id);
           
            return View(Feel);
        }

        // Delete Controller for Course list /

        public ActionResult Deletecl(int id)
        {
            var delete = db.Semester_Wise_Courses.Find(id);
            db.Semester_Wise_Courses.Remove(delete);
            db.SaveChanges();

            return RedirectToAction("CourseList");
        }
        // Semester Wise Courses List View

        public ActionResult CourseList()
        {
            var listswc = db.Semester_Wise_Courses.ToList();

            return View(listswc);
        }

        // Course List Edit 

        [HttpGet]
        public ActionResult EditCL(int id)
        {
            SemsterList(0);
            Semester_Wise_Courses listcl = db.Semester_Wise_Courses.Find(id);
            return View(listcl);
        }

        [HttpPost]

        public ActionResult EditCl(Semester_Wise_Courses cled)
        {
            
            db.Entry(cled).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("CourseList");
        }
        // Semester Wise Courses
        [HttpGet]
        public ActionResult AddCourse()
        {

            SemsterList(0);
            return View();
        }
        [HttpPost]
        public ActionResult AddCourse(Semester_Wise_Courses Swc)
        {
            db.Semester_Wise_Courses.Add(Swc);
            db.SaveChanges();

            return RedirectToAction("AddCourse");
        }

        // Index Page ( choice admin or student )

        public ActionResult Index()
        {
            return View();
        }

        // Admin MainPage
        public ActionResult MainPage()
        {

            // List Table Add Fees

            var feelist = db.Student.AsEnumerable()
                                    .Join
                                    (
                                        db.AddFees,
                                            s => new { s.StdId},
                                            a => new { a.StdId},
                                            (s, a) => new { s, a}
                                    )
                                    .Join
                                    (
                                        db.Department,
                                        s => new { s.s.SemesterId },
                                        d => new { d.SemesterId },
                                        (s, d) => new { s.s , s.a, d }
                                    )
                                    .Select
                                     (
                                         x => new AddFees
                                         {
                                             Add_Fee_Id     = x.a.Add_Fee_Id,
                                             StdRollNo      = x.a.StdRollNo,
                                             StdName        = x.a.StdName,
                                             Semester       = x.a.Semester,
                                             Payable_Fees   = x.a.Payable_Fees,
                                             DOT            = x.a.DOT,
                                             TotalFees      = x.a.TotalFees,
                                             Department     = x.d.SemesterList
                                         }
                                     )
                                     .OrderByDescending(x => x.Add_Fee_Id).Take(4)
                                     .ToList();

            int outstanding;
            int? paid;
            //Total Students
            ViewData["studentCount"] = db.Student.Count(); 

            //Total Courses
            ViewData["Course"] = db.Semester_Wise_Courses.Count();

            // Total New Students
            ViewData["NewStudent"] = db.Student.Where(x => x.Semester == 1).Count();

            //Total Outstanding
            outstanding = db.Student
                                .Join
                                (
                                    db.Semester_Wise_Courses, 
                                        s => new { s.SemesterId, s.Semester } , 
                                        c => new { SemesterId = c.DepartmentId, c.Semester} ,
                                        (s, c) => new { s, c}
                                )
                                .Sum(x => x.c.fees);

            ViewData["Total Fees"] = outstanding;

            paid = db.AddFees.Sum(x => (int?)x.Payable_Fees);
            ViewData["Pending"] = outstanding - paid ?? 0;



            return View(feelist);
        }

        // Email Check Exists or not

        public JsonResult check_email(string emailuser)
        {
            System.Threading.Thread.Sleep(200);
            var searchemail = db.Student.Where(x => x.Email == emailuser).SingleOrDefault();

            if (searchemail != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        // Registration View / 

        [HttpGet]
        public ActionResult Registration()
        {
            SemsterList(0);
            CampusList(0);

            return View();
        }
        [HttpPost]
        public ActionResult Registration(Student fees)
        {
            //int rollNo = db.Signup.Where(x => x.SemesterId == fees.SemesterId).Select(x => x.StdRollNo).Count();

            //fees.StdRollNo = rollNo + 1;

            SemsterList(0);
            CampusList(0);

            db.Student.Add(fees);
            db.SaveChanges();

            return RedirectToAction("StudentList");
        }

        // StudentList Controller / 

        [HttpGet]
        public ActionResult StudentList()
        {
            var data = db.Student.AsEnumerable()
                         .Join
                         (
                             db.Department,
                                a => a.SemesterId,
                                b => b.SemesterId, 
                                (a, b) => new { a, b }
                         )
                         .Join
                         (
                             db.Campus,
                                a => a.a.CampusId,
                                c => c.CampusId,
                                (a, c) => new { a.a, a.b, c }
                         )
                       .Select
                         (
                             x => new Student
                             {
                                 StdId        = x.a.StdId,
                                 StdRollNo    = x.a.StdRollNo,
                                 Email        = x.a.Email,
                                 StdFName     = x.a.StdFName,
                                 StdName      = x.a.StdName,
                                 StdPassword  = x.a.StdPassword,
                                 StdAge       = x.a.StdAge,
                                 StdAddress   = x.a.StdAddress,
                                 Gender       = x.a.Gender,
                                 Semester     = x.a.Semester,
                                 Department   = x.b.SemesterList,
                                 Campus       = x.c.CampusName     
                             }
                         )
                         .ToList();

            return View(data);
        }

        // Add Fees Controller // 

        [HttpGet]
        public ActionResult Add(int id)
        {
            int? fees, paidAmount;

            Student std = db.Student.Find(id);

            fees = db.Semester_Wise_Courses.Where(x => x.DepartmentId == std.SemesterId && x.Semester == std.Semester).Sum(x => (int?)x.fees);

            if (fees == null || fees == 0) { return View(); }

            paidAmount = db.AddFees.Where(x => x.StdId == std.StdId && x.Semester == std.Semester).Sum(x => (int?)x.Payable_Fees);

            std.paidAmount = paidAmount ?? 0;
            std.FeesAmount = 0;
            std.TotalFees  = fees ?? 0;
            std.RemainFees = std.TotalFees - std.paidAmount;
            //View
            return View(std);
        }
        [HttpPost]
        public ActionResult Add(Student stdllist)
        {
            AddFees ad = new AddFees
            {
                DOT             = DateTime.Now,
                Payable_Fees    = stdllist.FeesAmount,
                StdRollNo       = stdllist.StdRollNo,
                StdId           = stdllist.StdId,
                StdName         = stdllist.StdName,
                StdFName        = stdllist.StdFName,
                Email           = stdllist.Email,
                Semester        = stdllist.Semester,
                TotalFees       = stdllist.TotalFees,
            };

            db.AddFees.Add(ad);
            db.SaveChanges();

            return RedirectToAction("Add");

        }
       
        // Edit Student List
        [HttpGet]
        public ActionResult Edit(int id)
        {

            var stdllist = db.Student.Find(id);

            SemsterList(stdllist.SemesterId);

            CampusList(stdllist.CampusId);

            return View(stdllist);
        }
        [HttpPost]
        public ActionResult Edit(Student stdllist)
        {

            db.Entry(stdllist).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("StudentList");
        }
       
        // Delete Controller for student list /

        public ActionResult Delete(int delteid)
        {
            var delete = db.Student.Find(delteid);
            db.Student.Remove(delete);
            db.SaveChanges();

            return RedirectToAction("StudentList");
        }

        // Delete Controller for Admin Request ( AdRequest)  list /

        public ActionResult Deletee(int delteids)
        {
            var deletse = db.Request_Std.Find(delteids);
            db.Request_Std.Remove(deletse);
            db.SaveChanges();

            return RedirectToAction("AdRequest");
        }
       
        // private SemsterList
        private void SemsterList(int id)
        {
            var Semesterr = db.Department.ToList();
            ViewBag.SemesterList = new SelectList(Semesterr, "SemesterId", "SemesterList", id);
        }

        // private CampusList
        private void CampusList(int id)
        {
            var Campusllist = db.Campus.ToList();
            ViewBag.CampusList = new SelectList(Campusllist, "CampusId", "CampusName", id);

        }

        // private concession
        private void Concessionlist(int id)
        {
            var Concessiondropdown = db.concession_dropdwon.ToList();
            ViewBag.Concessionlist = new SelectList(Concessiondropdown, "concessionid", "Concessionlist", id);

        }

        public int RollNo(int Semesterid)
        {
            int rollNo = 0;

            var std = db.Student.Where(x => x.SemesterId == Semesterid).ToList();
            if (std.Count > 0) { rollNo = std.Max(x => x.StdRollNo); }

            return rollNo;
        }

        // Login Admin
        public ActionResult LoginAd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginAd(Admin_Login test)
        {
            var result = db.Admin_Login.Where(x => x.AdEmail == test.AdEmail && x.AdPassword == test.AdPassword).FirstOrDefault();
            if (result != null)
            {
                return RedirectToAction("MainPage", "Home");
            }
            else
            {
                ViewData["loginflag"] = "Invalid Enter Details";
                return View();
            }
        }

        // Student Login
        public ActionResult StdLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StdLogin(Student objuser)
        {
           
            var result = db.Student.Where(x => x.Email == objuser.Email && x.StdPassword == objuser.StdPassword).FirstOrDefault();
            if (result != null)
            {
                string uer = User.Identity.Name;
                FeeCollection(result);
                return RedirectToAction("StdMain", "Home");
            }
            else
            {
                ViewData["loginflag"] = "Invalid Student";
                return View();
            }
        }

        
        // View Request Admin 
        [HttpGet]
        public ActionResult AdRequest()
        {
            var datareuest = db.Request_Std.AsEnumerable()
                        .Join
                        (
                            db.Department,
                               x => x.SemesterId,
                               y => y.SemesterId,
                               (x, y) => new { x, y }
                        )
                        .Join
                        (
                            db.Campus,
                               x => x.x.CampusId,
                               z => z.CampusId,
                               (x, z) => new { x.x, x.y, z }
                        )
                        .Join
                        (
                            db.concession_dropdwon,
                            x => x.x.concessionid,
                            l => l.concessionid,
                            (x, l) => new {x.x ,x.y , x.z , l}
                        )
                      .Select
                        (
                            xx => new Request_Std
                            {
                                Department  = xx.y.SemesterList,
                                Campus      = xx.z.CampusName,
                                concessionn = xx.l.Concessionlist,
                                StdEmail    = xx.x.StdEmail,
                                Description = xx.x.Description
                            }
                        )
                        .ToList();

            return View(datareuest);
        }

        // Add Fees Select Jidhr sy fees generate ho gai...
        public ActionResult AddFeeSelect() 
        {
                
                var data = db.Student.AsEnumerable()
                             .Join
                             (
                                 db.Department,
                                    a => a.SemesterId,
                                    b => b.SemesterId,
                                    (a, b) => new { a, b }
                             )
                             .Join
                             (
                                 db.Campus,
                                    a => a.a.CampusId,
                                    c => c.CampusId,
                                    (a, c) => new { a.a, a.b, c }
                             )
                           .Select
                             (
                                 x => new Student
                                 {
                                     StdId = x.a.StdId,
                                     StdRollNo = x.a.StdRollNo,
                                     Email = x.a.Email,
                                     StdFName = x.a.StdFName,
                                     StdName = x.a.StdName,
                                     StdPassword = x.a.StdPassword,
                                     StdAge = x.a.StdAge,
                                     StdAddress = x.a.StdAddress,
                                     Gender = x.a.Gender,
                                     Semester = x.a.Semester,
                                     Department = x.b.SemesterList,
                                     Campus = x.c.CampusName
                                 }
                             )
                             .ToList();

                return View(data);
            
        }

        // -------------------- Student Work Start ----------------- //

        // Student Main Page
        public ActionResult StdMain()
        {
            //Total Courses
       
            return View();
        }

        // Student Fee List
        public ActionResult StdFeeList()
        {
            return View();
        }

        public ActionResult PartialFeelist( bool recentpaid , int id)
        {
            string recentFlag = "No";

            List<AddFees> fees = new List<AddFees>();
            if (recentpaid)
            {
                recentFlag = "Yes";
                fees = db.AddFees.Where(x => x.StdId == id).OrderByDescending(x => x.StdId).Take(4).ToList();
            }
            else
            {
                fees = db.AddFees.Where(x => x.StdId == id).ToList();
            }

            ViewData["Recentpaid"] = recentFlag;

            return PartialView(fees);

        }
        // Request Concession Student
        public ActionResult concession()
        {
            SemsterList(0);
            CampusList(0);
            Concessionlist(0);

            return View();
        }
        [HttpPost]
        public ActionResult concession(Request_Std request)
        {
            SemsterList(0);
            CampusList(0);
            Concessionlist(0);

            db.Request_Std.Add(request);
            db.SaveChanges();
            return RedirectToAction("StdMain");
        }
        [HttpGet]
        public ActionResult StdProfile(int id)
        {

            var stdllist = db.Student.Find(id);

            SemsterList(stdllist.SemesterId);

            CampusList(stdllist.CampusId);

            return View(stdllist);
        }
        [HttpGet]
        public ActionResult Stdsmstrdetails(int id)
        {

            var stdsmstdtl = db.Student.Find(id);

            SemsterList(stdsmstdtl.SemesterId);

            CampusList(stdsmstdtl.CampusId);

            return View(stdsmstdtl);
        }

        private void FeeCollection(Student std)
        {
            int outstanding;
            int? paid;

            //Student Name
            Session["studentName"]  = std.StdName;
            //Student Id
            Session["studentId"]    = std.StdId;
            //Semester
            Session["Semester"]     = std.Semester;
            //Course
            Session["Course"]       = db.Semester_Wise_Courses.Where(x => x.DepartmentId == std.SemesterId && x.Semester == std.Semester).Count();

            //Total Outstanding
            outstanding = db.Semester_Wise_Courses.Where(x => x.DepartmentId == std.SemesterId && x.Semester == std.Semester).Sum(x => x.fees);
            Session["TotalFees"] = outstanding;

            //Paid
            paid = db.AddFees.Where(x => x.StdId == std.StdId && x.Semester == std.Semester).Sum(x => (int?)x.Payable_Fees);
            Session["pending"] = outstanding - paid ?? 0;
        }
    }
} 
    
