using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FeeManagement.Models
{
    public class BSCS_Seven
    {
        [Key]
        public int BSCS_Seven_ID { get; set; }
        public string Seven_cn { get; set; }
        public int Seven_credit_hours { get; set; }
        public int Seven_fees { get; set; }
    }
}