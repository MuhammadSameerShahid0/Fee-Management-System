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

        // GET: Home
        public ActionResult MainPage()
        {
    
            // List Table Add Fees
            var feelist = db.AddFees.ToList().OrderByDescending(x => x.Add_Fee_Id).Take(4);

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

        // Registration Controller / 

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
            //dynamic modelstdlist = new ExpandoObject();
            //modelstdlist.FeeManagementSignupDropdown = GetFeeManagementSignupDropdown();
            //modelstdlist.FeeManagementSignup = GetFeeManagementSignup();
            //return View(modelstdlist);

            //Semester 
            //Student 
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

            //var datajoin = db.AddFees.AsEnumerable()
            //    .Join
            //    (
            //        db.Student,
            //        x => x.StdId,
            //        c => c.StdId,
            //        (x, c) => new { x, c }
            //    )
            //    .Select
            //    (
            //        l => new AddFees
            //        {
            //            StdId = l.x.StdId,
            //            StdName = l.x.StdName,
            //            StdFName = l.x.StdFName,
            //            StdRollNo = l.x.StdRollNo,
            //            Add_Fee_Id = l.x.Add_Fee_Id,
            //            Invoice_ID = l.x.Invoice_ID,
            //            DOT = l.x.DOT,
            //            Payable_Fees = l.x.Payable_Fees

            //        }
            //    ).ToList();

            //return View(datajoin);
        }
       
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

        // Edit Controller for BSCS First/ 

        [HttpGet]
        public ActionResult EditBSCSFirst(int ids)
        {

            var editbscsfirst = db.BSCS_First_d.Find(ids);

            return View(editbscsfirst);
        }
        [HttpPost]
        public ActionResult EditBSCSFirst(BSCS_First editbscsfirst)
        {

            db.Entry(editbscsfirst).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("BscsFirst");
        }

        // Edit Controller for BSCS Second/ 

        [HttpGet]
        public ActionResult EditBSCSSecond(int idsecond)
        {

            var editbscssecond = db.BSCS_Second_d.Find(idsecond);

            return View(editbscssecond);
        }
        [HttpPost]
        public ActionResult EditBSCSSecond(BSCS_Second editbscssecond)
        {

            db.Entry(editbscssecond).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("BscsSecond");
        }

        // Edit Controller for BSCS Third/ 

        [HttpGet]
        public ActionResult EditBSCSThird(int idthird)
        {

            var editbscsthird = db.BSCS_Third_d.Find(idthird);

            return View(editbscsthird);
        }
        [HttpPost]
        public ActionResult EditBSCSThird(BSCS_Third editbscsthird)
        {

            db.Entry(editbscsthird).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("BscsThird");
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
        // Delete Controller for BSCS First list /

        public ActionResult DeleteBSCSFirst(int delteide)
        {
            var deletee = db.BSCS_First_d.Find(delteide);
            db.BSCS_First_d.Remove(deletee);
            db.SaveChanges();

            return RedirectToAction("BscsFirst");
        }
        // Delete Controller for BSCS Second list /

        public ActionResult DeleteBSCSSecond(int delteidesecond)
        {
            var deletees = db.BSCS_Second_d.Find(delteidesecond);
            db.BSCS_Second_d.Remove(deletees);
            db.SaveChanges();

            return RedirectToAction("BscsSecond");
        }
        // Delete Controller for BSCS Third list /

        public ActionResult DeleteBSCSThird(int delteidethird)
        {
            var deleteess = db.BSCS_Third_d.Find(delteidethird);
            db.BSCS_Third_d.Remove(deleteess);
            db.SaveChanges();

            return RedirectToAction("BscsThird");
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
                Session["studentName"]  = result.StdName;
                Session["studentId"]    = result.StdId;


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

        // Add Fees Select Button Sarein  BSCS BSEE etc...
        public ActionResult AddFeeSelect() 
        {
                //dynamic modelstdlist = new ExpandoObject();
                //modelstdlist.FeeManagementSignupDropdown = GetFeeManagementSignupDropdown();
                //modelstdlist.FeeManagementSignup = GetFeeManagementSignup();
                //return View(modelstdlist);

                //Semester 
                //Student 
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

        // BSCS View 
        public ActionResult Bscsview()
        {
            return View();
        }

        // BSCS 1st 
        public ActionResult BscsFirst()
        {
            List<BSCS_First> BSCSfirst = db.BSCS_First_d.ToList();
            return View(BSCSfirst);
        }

        // Add Course Bscs First/ 

        [HttpGet]
        public ActionResult AddBscsFirst()
        {

            return View();
        }
        [HttpPost]
        public ActionResult AddBscsFirst(BSCS_First AddBscsFirsts)
        {

            db.BSCS_First_d.Add(AddBscsFirsts);
            db.SaveChanges();

            return RedirectToAction("BscsFirst");
        }


        // BSCS 2nd 
        public ActionResult BscsSecond()
        {
            List<BSCS_Second> BSCSSecond = db.BSCS_Second_d.ToList();
            return View(BSCSSecond);
        }

        // Add Course Bscs First/ 

        [HttpGet]
        public ActionResult AddBscsSecond()
        {

            return View();
        }
        [HttpPost]
        public ActionResult AddBscsSecond(BSCS_Second AddBscsSecond)
        {

            db.BSCS_Second_d.Add(AddBscsSecond);
            db.SaveChanges();

            return RedirectToAction("BscsSecond");
        }

        // BSCS 3rd 
        public ActionResult BscsThird()
        {
            List<BSCS_Third> BSCSThird = db.BSCS_Third_d.ToList();
            return View(BSCSThird);
        }

        // Add Course Bscs First/ 

        [HttpGet]
        public ActionResult AddBscsThird()
        {

            return View();
        }
        [HttpPost]
        public ActionResult AddBscsThird(BSCS_Third AddBscsThird)
        {

            db.BSCS_Third_d.Add(AddBscsThird);
            db.SaveChanges();

            return RedirectToAction("BscsThird");
        }




        // -------------------- Student Work Start ----------------- //

        // Student Main Page
        public ActionResult StdMain()
        {
            return View();
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

    }
} 
    
