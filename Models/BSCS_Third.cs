using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FeeManagement.Models
{
    public class BSCS_Third
    {
        [Key]
        public int BSCS_Third_ID { get; set; }
        public string Third_cn { get; set; }
        public int Third_credit_hours { get; set; }
        public int Third_fees { get; set; }
    }
}