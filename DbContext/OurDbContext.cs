using FeeManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FeeManagement
{
    public class OurDbContext: DbContext
    {
        #region Constructor
        public OurDbContext() : base("FYPFeesManagement")
        {
            
        }
        #endregion
        public DbSet<Student> Student                             { get; set; }
        public DbSet<Department> Department                       { get; set; }
        public DbSet<Campus> Campus                               { get; set; }
        public DbSet<Admin_Login> Admin_Login                     { get; set; }
        public DbSet<Fees_Management> Fees_Management             { get; set; }
        public DbSet<Request_Std> Request_Std                     { get; set; }
        public DbSet<concession_dropdwon> concession_dropdwon     { get; set; }
        public DbSet<AddFees> AddFees                             { get; set; }        
        public DbSet<Semester_Wise_Courses> Semester_Wise_Courses { get; set; }        
        public DbSet<EnrollStd> Enroll_Student                    { get; set; }        
        public DbSet<EnrollFee> Enroll_Student_Fee                { get; set; }        
    }
}