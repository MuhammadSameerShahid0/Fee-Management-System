using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FeeManagement.Models
{
    public class Campus
    {
        [Key]
        public int CampusId { get; set; }
        public string CampusName{ get; set; }

    }
}