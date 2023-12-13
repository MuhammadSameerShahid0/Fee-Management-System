using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FeeManagement.Models
{
    public class BSCS_Fourth
    {
        [Key]
        public int BSCS_Fourth_ID { get; set; }
        public string Fourth_cn { get; set; }
        public int Fourth_credit_hours { get; set; }
        public int Fourth_fees { get; set; }
    }
}