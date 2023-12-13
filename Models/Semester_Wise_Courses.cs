using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FeeManagement.Models
{
    public class Semester_Wise_Courses
    {
        [Key]
        public int Id           { get; set; }
        public int DepartmentId { get; set; }
        public int Semester     { get; set; }
        public string course_name   { get; set; }
        public int credit_hours { get; set; }
        public int fees         { get; set; }
    }
}