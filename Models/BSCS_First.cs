using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace FeeManagement.Models
{
    public class BSCS_First
    {
        [Key]
        public int BSCS_First_ID        { get; set; }
        public string first_cn          { get; set; }
        public int first_credit_hours   { get; set; }
        public int first_fees           { get; set; }

    }
}