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
using System.Web.UI;
using System.Web.WebPages;

namespace FeeManagement.Controllers
{
    public class HomeController : Controller
    {
        OurDbContext db = new OurDbContext();

        // Enroll Student FeeList Admin
        public ActionResult EnrollFeeList()
        {
            var x = db.Enroll_Student_Fee.OrderByDescending(xx => xx.id).ToList();
            return View(x);
        }
        // Enroll FeeList Admin jidhr agy jana gerate fee button wala page
        public ActionResult EnrollFee()
        {
            var efl = db.Enroll_Student.OrderByDescending(x => x.id).ToList();

            return View(efl);
        }

        // Enroll List Admin Page
        public ActionResult Enrolllist()
        {
            var efl = db.Enroll_Student.OrderByDescending(x => x.id).ToList();

            return View(efl);
        }
        // Fees List Admin //

        public ActionResult FeeList()
        {
            var Feel = db.Student.AsEnumerable()
                                    .Join
                                    (
                                        db.AddFees,
                                            s => new { s.StdId },
                                            a => new { a.StdId },
                                            (s, a) => new { s, a }
                                    )
                                    .Join
                                    (
                                        db.Department,
                                        s => new { s.s.SemesterId },
                                        d => new { d.SemesterId },
                                        (s, d) => new { s.s, s.a, d }
                                    )
                                    .Select
                                     (
                                         x => new AddFees
                                         {
                                             Add_Fee_Id = x.a.Add_Fee_Id,
                                             StdRollNo = x.a.StdRollNo,
                                             StdName = x.a.StdName,
                                             Semester = x.a.Semester,
                                             Payable_Fees = x.a.Payable_Fees,
                                             DOT = x.a.DOT,
                                             TotalFees = x.a.TotalFees,
                                             Department = x.d.SemesterList
                                         }
                                     ).ToList()
                                     .OrderByDescending(x => x.Add_Fee_Id);

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
                                            s => new { s.StdId },
                                            a => new { a.StdId },
                                            (s, a) => new { s, a }
                                    )
                                    .Join
                                    (
                                        db.Department,
                                        s => new { s.s.SemesterId },
                                        d => new { d.SemesterId },
                                        (s, d) => new { s.s, s.a, d }
                                    )
                                    .Select
                                     (
                                         x => new AddFees
                                         {
                                             Add_Fee_Id = x.a.Add_Fee_Id,
                                             StdRollNo = x.a.StdRollNo,
                                             StdName = x.a.StdName,
                                             Semester = x.a.Semester,
                                             Payable_Fees = x.a.Payable_Fees,
                                             DOT = x.a.DOT,
                                             TotalFees = x.a.TotalFees,
                                             Department = x.d.SemesterList
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
                                        s => new { s.SemesterId, s.Semester },
                                        c => new { SemesterId = c.DepartmentId, c.Semester },
                                        (s, c) => new { s, c }
                                )
                                .Sum(x => x.c.fees);

            ViewData["Total Fees"] = outstanding;

            paid = db.AddFees.Sum(x => (int?)x.Payable_Fees);
            ViewData["Pending"] = outstanding - paid ?? 0;

            ViewData["EnrollCourse"] = db.Enroll_Student.Count();


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
                         ).OrderByDescending(x => x.StdId)
                         .ToList();

            return View(data);
        }

        // enroll fee generate 
        [HttpGet]
        public ActionResult EnrollFeeGen(int id)
        {
            var x = db.Enroll_Student.Find(id);
      
            return View(x);
        }

        [HttpPost]
        public ActionResult EnrollFeeGen(EnrollStd ed)
        {
            EnrollFee ef = new EnrollFee
            {
                Dot = DateTime.Now,
                StdId = ed.StdId,
                StdRollNo = ed.StdRollNo,
                StdName = ed.StdName,
                Email = ed.Email,
                Semester = ed.Semester,
                course_name = ed.course_name,
                credit_hours = ed.credit_hours,
                fees = ed.fees,
                Current_Semester = ed.Current_Semester,
                payable_fees = ed.payable_fees

            };

            db.Enroll_Student_Fee.Add(ef);
            db.SaveChanges();
            return RedirectToAction("EnrollFeeList");
        }

        // Add Fees Controller // 

        [HttpGet]
        public ActionResult Add(int id)
        {
            int? fees, paidAmount  /*previousremain*/ ;

            Student std = db.Student.Find(id);

            fees = db.Semester_Wise_Courses.Where(x => x.DepartmentId == std.SemesterId && x.Semester == std.Semester).Sum(x => (int?)x.fees);

            if (fees == null || fees == 0) { return View(); }

            paidAmount = db.AddFees.Where(x => x.StdId == std.StdId && x.Semester == std.Semester).Sum(x => (int?)x.Payable_Fees);

            std.paidAmount = paidAmount ?? 0;
            std.FeesAmount = 0;
            std.TotalFees = fees ?? 0;
            std.RemainFees = std.TotalFees - std.paidAmount;

            // proper kam ni hy yh

            //Min(x => (int?)x.RemainFees - (int?)x.Payable_Fees)
            //previousremain = db.AddFees.Where(x => x.StdId == std.StdId && x.Semester != std.Semester).Sum(x => x.Payable_Fees);
       
            //std.previousRemainFees = std.TotalFees - previousremain ?? 0 ;
           

            //View
            return View(std);
        }
        [HttpPost]
        public ActionResult Add(Student stdllist)
        {
            AddFees ad = new AddFees
            {
                DOT = DateTime.Now,
                Payable_Fees = stdllist.FeesAmount,
                StdRollNo = stdllist.StdRollNo,
                StdId = stdllist.StdId,
                StdName = stdllist.StdName,
                StdFName = stdllist.StdFName,
                Email = stdllist.Email,
                Semester = stdllist.Semester,
                TotalFees = stdllist.TotalFees,
                RemainFees = stdllist.RemainFees,
                previousRemainFees = stdllist.previousRemainFees
                
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
                            (x, l) => new { x.x, x.y, x.z, l }
                        )
                      .Select
                        (
                            xx => new Request_Std
                            {
                                Department = xx.y.SemesterList,
                                Campus = xx.z.CampusName,
                                concessionn = xx.l.Concessionlist,
                                StdEmail = xx.x.StdEmail,
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
                         ).OrderByDescending(x => x.StdId)
                         .ToList();

            return View(data);

        }

        // -------------------- Student Work Start ----------------- //

        //Enroll Fee Student
        public ActionResult EnrollFeeListstd(int id)
        {
            List<EnrollFee> fees = new List<EnrollFee>();

            fees = db.Enroll_Student_Fee.Where(x => x.StdId == id).OrderByDescending(x => x.StdId).ToList();
            return View(fees);
        }

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
        // StdFeelist or STUDENT Main my dala hy ya.
        public ActionResult PartialFeelist(bool recentpaid, int id)
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

        //  Partial Course Student Page
        public ActionResult PartialCourse(int id)
        {
            Student std = db.Student.Find(id);
            List<Semester_Wise_Courses> course = new List<Semester_Wise_Courses>();

            course = db.Semester_Wise_Courses.Where(x => x.DepartmentId == std.SemesterId && x.Semester == std.Semester).ToList();

            return PartialView(course);
        }

        // Course Details Student Page
        public ActionResult CourseDetail()
        {
            return View();
        }

        // Enroll course list student page
        public ActionResult Enrollment(int id)
        {
            Student std = db.Student.Find(id);
            List<Semester_Wise_Courses> course = new List<Semester_Wise_Courses>();

            course = db.Semester_Wise_Courses.Where(x => x.DepartmentId == std.SemesterId && x.Semester != std.Semester).ToList();

            return View(course);
        }

        // EnrollAdd student page jidhr sy student enroll krny ky liya subject select krta. wo button hy yh.
        [HttpGet]
        public ActionResult EnrollAdd(int id)
        {

            Semester_Wise_Courses swss = db.Semester_Wise_Courses.Find(id);

            return View(swss);
        }
        [HttpPost]
        public ActionResult EnrollAdd(Student std , Semester_Wise_Courses sws)
        {
            EnrollStd esd = new EnrollStd
            {
                Dot              = DateTime.Now,
                StdId            = std.StdId,
                StdRollNo        = std.StdRollNo,
                Email            = sws.Email,
                StdName          = sws.StdName,
                course_name      = sws.course_name,
                credit_hours     = sws.credit_hours,
                fees             = sws.fees,
                Semester         = sws.Semester,
                Current_Semester = sws.Current_Semester

            };

            db.Enroll_Student.Add(esd);
            db.SaveChanges();

            return RedirectToAction("StdMain");
        }
        //View Enroll Course Student
        public ActionResult ViewEnrollStd(int id)
        {
            Student std = db.Student.Find(id);
            List<EnrollStd> enrollstdview = new List<EnrollStd>();

            enrollstdview = db.Enroll_Student.Where(x => x.StdId == std.StdId && x.StdRollNo == std.StdRollNo).OrderByDescending(x => x.id).ToList();

            return View(enrollstdview);
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

        // Invoice Student Fee
        public ActionResult InvoiceFe(int id)
        {
            var invoicefee = db.AddFees.Find(id);
            return View(invoicefee);
        }

        // Invoice Enroll Fee
        public ActionResult EnrollInvoiceFe(int id)
        {
            var enrollinvoicefee = db.Enroll_Student_Fee.Find(id);
            return View(enrollinvoicefee);
        }
        private void FeeCollection(Student std)
        {
            int outstanding;
            int? paid;
            //Student Email
            Session["studentEmail"] = std.Email;
            //Student Rollno
            Session["studentRollno"] = std.StdRollNo;
            //Student Name
            Session["studentName"] = std.StdName;
            //Student Semester
            Session["studentSemester"] = std.Semester;
            //Student Id
            Session["studentId"] = std.StdId;
            //Semester
            Session["Semester"] = std.Semester;
            //Course
            Session["Course"] = db.Semester_Wise_Courses.Where(x => x.DepartmentId == std.SemesterId && x.Semester == std.Semester).Count();
            //Total Outstanding
            outstanding = db.Semester_Wise_Courses.Where(x => x.DepartmentId == std.SemesterId && x.Semester == std.Semester).Sum(x => x.fees);
            Session["TotalFees"] = outstanding;

            //Paid
            paid = db.AddFees.Where(x => x.StdId == std.StdId && x.Semester == std.Semester).Sum(x => (int?)x.Payable_Fees);
            Session["pending"] = outstanding - paid ?? 0;
        }
    }
}
