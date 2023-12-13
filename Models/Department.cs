using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FeeManagement.Models
{
    public class Department
    {
        [Key]
        public int SemesterId            { get; set; }
        public string SemesterList       { get; set; }
    }
}