using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FeeManagement.Models
{
    public class Student
    {
        [Key]
        public int StdId            { get; set; }
        public int StdRollNo        { get; set; }
        public string StdPassword   { get; set; }
        public string StdName       { get; set; }
        public string StdFName      { get; set; }
        public string StdAge        { get; set; }
        public string StdAddress    { get; set; }
        public string Email         { get; set; }
        public string Gender        { get; set; } 
        public int SemesterId       { get; set; }
        public int Semester         { get; set; }
        public int CampusId         { get; set; }


        [NotMapped]
        public string Department    { get; set; }
        [NotMapped]
        public string Campus        { get; set; }
        
        [NotMapped]
        public int FeesAmount       { get; set; }
        [NotMapped]
        public int paidAmount       { get; set; }
        [NotMapped]
        public int TotalFees { get; set; }
        [NotMapped]
        public int RemainFees { get; set; }
    }
}