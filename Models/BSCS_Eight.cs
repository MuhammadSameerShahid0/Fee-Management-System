using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace FeeManagement.Models
{
    public class BSCS_Eight
    {
        [Key]
        public int BSCS_Eight_ID { get; set; }
        public string Eight_cn { get; set; }
        public int Eight_credit_hours { get; set; }
        public int Eight_fees { get; set; }
    }
}