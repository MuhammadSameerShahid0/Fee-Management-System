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
        public DbSet<Student> Student                            { get; set; }
        public DbSet<Department> Department                      { get; set; }
        public DbSet<Campus> Campus                              { get; set; }
        public DbSet<Admin_Login> Admin_Login                    { get; set; }
        public DbSet<Fees_Management> Fees_Management            { get; set; }
        public DbSet<Request_Std> Request_Std                    { get; set; }
        public DbSet<concession_dropdwon> concession_dropdwon    { get; set; }
        public DbSet<BSCS_Details> Bscs_details                  { get; set; }
        public DbSet<BSCS_First> BSCS_First_d                    { get; set; }
        public DbSet<BSCS_Second> BSCS_Second_d                  { get; set; }
        public DbSet<BSCS_Third> BSCS_Third_d                    { get; set; }
        public DbSet<BSCS_Fourth> BSCS_Fourth_d                  { get; set; }
        public DbSet<BSCS_Fifth> BSCS_Fifth_d                    { get; set; }
        public DbSet<BSCS_Six> BSCS_Six_d                        { get; set; }
        public DbSet<BSCS_Seven> BSCS_Seven_d                    { get; set; }
        public DbSet<BSCS_Eight> BSCS_Eight_d                    { get; set; }
        public DbSet<AddFees> AddFees                            { get; set; }        
        public DbSet<Semester_Wise_Courses> Semester_Wise_Courses { get; set; }        
    }
}