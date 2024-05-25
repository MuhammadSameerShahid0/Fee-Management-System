using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FeeManagement.Models
{
    public class EnrollStd
    {
        [Key]
        public int id { get; set; }
        public int StdId { get; set; }
        public int StdRollNo { get; set; }
        public string StdName { get; set; }
        public string Email { get; set; }
        public int Semester { get; set; }
        public string course_name { get; set; }
        public int credit_hours { get; set; }
        public int fees { get; set; }
        public int  Current_Semester { get; set; }
        public DateTime Dot { get; set; }

        [NotMapped]
        public int payable_fees { get; set; }
    }
}