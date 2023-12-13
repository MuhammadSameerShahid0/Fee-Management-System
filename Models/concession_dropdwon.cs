using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FeeManagement.Models
{
    public class concession_dropdwon
    {
        [Key]
        public int concessionid         { get; set; }
        public string Concessionlist   { get; set; }
    }
}