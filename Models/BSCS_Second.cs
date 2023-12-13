using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FeeManagement.Models
{
    public class BSCS_Second
    {
        [Key]
        public int BSCS_Second_ID { get; set; }
        public string Second_cn { get; set; }
        public int Second_credit_hours { get; set; }
        public int Second_fees { get; set; }
    }
}