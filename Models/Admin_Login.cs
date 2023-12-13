using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FeeManagement.Models
{
    public class Admin_Login
    {
        [Key]
        public int AdId             { get; set; }
        public string AdEmail       { get; set; }
        public string AdPassword    { get; set; }
    }
}